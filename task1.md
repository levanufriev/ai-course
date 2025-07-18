You need to write xUnit tests for the OrderService.CalculateOrder method. Follow the same style as the existing ShippingServiceTests example. Make sure to cover all important cases:
- Test that the method throws an ArgumentException when totalAmount is negative.
- Test the discount logic for members (isMember = true), checking both cases where totalAmount is above and below 1000.
- Test the discount logic for non-members (isMember = false), checking both cases where totalAmount is above and below 5000.
- Verify the bonus points calculation for both members and non-members.
Use [Fact] for single tests and [Theory] with [InlineData] when testing multiple inputs. Follow the Arrange-Act-Assert pattern and use FluentAssertions for clear checks. Name your test methods descriptively, like CalculateOrder_Gives15PercentDiscount_WhenMemberAndAmountOver1000.
Place the test class in the Task1.Tests.Services namespace, just like the example.
