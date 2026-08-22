using Libro.Services;
using Xunit;

namespace Libro.Tests
{
    public class FineCalculatorTests
    {
        [Fact]
        public void Calculate_ReturnedOnTime_ReturnsNull()
        {
            // Arrange
            var dueDate = new DateTime(2026, 1, 10);
            var returnDate = new DateTime(2026, 1, 10);

            // Act
            var fine = FineCalculator.Calculate(dueDate, returnDate);

            // Assert
            Assert.Null(fine);
        }

        [Fact]
        public void Calculate_ReturnedEarly_ReturnsNull()
        {
            var dueDate = new DateTime(2026, 1, 10);
            var returnDate = new DateTime(2026, 1, 8);

            var fine = FineCalculator.Calculate(dueDate, returnDate);

            Assert.Null(fine);
        }

        [Fact]
        public void Calculate_OneDayLate_Returns25()
        {
            var dueDate = new DateTime(2026, 1, 10);
            var returnDate = new DateTime(2026, 1, 11);

            var fine = FineCalculator.Calculate(dueDate, returnDate);

            Assert.Equal(25m, fine);
        }

        [Fact]
        public void Calculate_ThreeDaysLate_Returns75()
        {
            var dueDate = new DateTime(2026, 1, 10);
            var returnDate = new DateTime(2026, 1, 13);

            var fine = FineCalculator.Calculate(dueDate, returnDate);

            Assert.Equal(75m, fine);
        }

        [Theory]
        [InlineData(1, 25)]
        [InlineData(2, 50)]
        [InlineData(10, 250)]
        public void Calculate_VariousDaysLate_ReturnsCorrectFine(int daysLate, decimal expectedFine)
        {
            var dueDate = new DateTime(2026, 1, 1);
            var returnDate = dueDate.AddDays(daysLate);

            var fine = FineCalculator.Calculate(dueDate, returnDate);

            Assert.Equal(expectedFine, fine);
        }
    }
}