namespace Task1.Tests.Services
{
    public class OrderServiceTests
    {
        [Fact]
        public void CalculateOrder_ThrowsArgumentException_WhenTotalAmountIsNegative()
        {
            // Arrange
            var service = new OrderService();
            int negativeAmount = -100;
            bool isMember = true;
            int itemsCount = 5;

            // Act
            Action act = () => service.CalculateOrder(negativeAmount, isMember, itemsCount);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Amount must be non-negative*")
                .WithParameterName("totalAmount");
        }

        [Theory]
        [InlineData(1200, true, 15, 1020, 24)] // Member, amount > 1000
        [InlineData(800, true, 10, 720, 16)]   // Member, amount <= 1000
        public void CalculateOrder_AppliesCorrectDiscountForMembers(
            int totalAmount, bool isMember, 
            int expectedDiscount, int expectedFinalAmount, int expectedBonusPoints)
        {
            // Arrange
            var service = new OrderService();
            int itemsCount = 5;

            // Act
            var result = service.CalculateOrder(totalAmount, isMember, itemsCount);

            // Assert
            result.DiscountPercent.Should().Be(expectedDiscount);
            result.FinalAmount.Should().Be(expectedFinalAmount);
            result.BonusPoints.Should().Be(expectedBonusPoints);
        }

        [Theory]
        [InlineData(6000, false, 5, 5700, 60)] // Non-member, amount > 5000
        [InlineData(4000, false, 0, 4000, 40)] // Non-member, amount <= 5000
        public void CalculateOrder_AppliesCorrectDiscountForNonMembers(
            int totalAmount, bool isMember,
            int expectedDiscount, int expectedFinalAmount, int expectedBonusPoints)
        {
            // Arrange
            var service = new OrderService();
            int itemsCount = 5;

            // Act
            var result = service.CalculateOrder(totalAmount, isMember, itemsCount);

            // Assert
            result.DiscountPercent.Should().Be(expectedDiscount);
            result.FinalAmount.Should().Be(expectedFinalAmount);
            result.BonusPoints.Should().Be(expectedBonusPoints);
        }

        [Theory]
        [InlineData(1000, true, 20)]  // Member: 1000/100*2
        [InlineData(1000, false, 10)] // Non-member: 1000/100*1
        public void CalculateOrder_CalculatesCorrectBonusPoints(
            int totalAmount, bool isMember, int expectedBonusPoints)
        {
            // Arrange
            var service = new OrderService();
            int itemsCount = 5;

            // Act
            var result = service.CalculateOrder(totalAmount, isMember, itemsCount);

            // Assert
            result.BonusPoints.Should().Be(expectedBonusPoints);
        }
    }
}
