You are working on the PaymentProcessingService in a C# project. Fix these two specific problems:
1. The service crashes with NullReferenceException when the payment gateway returns empty or invalid JSON
2. There's no retry logic when the gateway temporarily fails

Required fixes:

- Wrap the JSON deserialization in proper try-catch blocks
- Add null checks for the gateway response
- Implement retry logic with exponential backoff
- Keep all existing functionality
- Maintain current logging
- Follow the project's code style

Show the complete fixed ProcessPaymentAsync method with these changes. Include:

- The Polly retry policy setup
- Safe JSON deserialization
- Proper error handling
- All necessary using directives

After the code, briefly explain what you changed in 2-3 sentences.
