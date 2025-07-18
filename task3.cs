using Polly;
using Polly.Retry;
using System.Net;

public async Task<PaymentResult> ProcessPaymentAsync(int accountId, decimal amount)
{
    _logger.LogInformation("Start processing payment for account {AccountId}, amount {Amount}", accountId, amount);

    // 1) Check cache
    if (!_accounts.TryGetValue(accountId, out var account))
    {
        _logger.LogError("Account {AccountId} not found in cache", accountId);
        return PaymentResult.Failed("Account not found");
    }

    // Setup retry policy with exponential backoff
    var retryPolicy = RetryPolicy<HttpResponseMessage>
        .Handle<HttpRequestException>()
        .OrResult(r => !r.IsSuccessStatusCode)
        .WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, delay, retryCount, context) => 
            {
                _logger.LogWarning($"Attempt {retryCount}: Delaying for {delay.TotalSeconds} seconds due to: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
            });

    try
    {
        var request = new
        {
            AccountId = account.Id,
            Amount = amount
        };

        // 2) Send request to payment service with retry policy
        var response = await retryPolicy.ExecuteAsync(async () =>
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(request), 
                System.Text.Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(
                "https://api.payment-gateway.com/v1/payments", 
                requestContent);
        });

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Payment gateway returned status {StatusCode}", response.StatusCode);
            return PaymentResult.Failed("Gateway error");
        }

        var json = await response.Content.ReadAsStringAsync();
        GatewayResponse gatewayResult = null;

        try
        {
            gatewayResult = JsonSerializer.Deserialize<GatewayResponse>(json);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize gateway response");
            return PaymentResult.Failed("Invalid gateway response");
        }

        if (gatewayResult == null)
        {
            _logger.LogWarning("Payment gateway returned empty response");
            return PaymentResult.Failed("Empty gateway response");
        }

        if (!gatewayResult.Success)
        {
            _logger.LogWarning("Payment gateway failed: {Message}", gatewayResult.Message ?? "No error message provided");
            return PaymentResult.Failed("Gateway failure: " + gatewayResult.Message);
        }

        // 3) Correct balance locally
        if (amount < 0)
        {
            _logger.LogWarning("Negative payment amount: {Amount}", amount);
            return PaymentResult.Failed("Amount must be non-negative");
        }

        if (account.Balance < amount)
        {
            _logger.LogWarning("Insufficient funds for account {AccountId}", accountId);
            return PaymentResult.Failed("Insufficient funds");
        }

        account.Balance -= amount;
        _logger.LogInformation("Payment successful. New balance: {Balance}", account.Balance);

        return PaymentResult.Success(account.Balance);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error processing payment");
        return PaymentResult.Failed("Unexpected error");
    }
}
