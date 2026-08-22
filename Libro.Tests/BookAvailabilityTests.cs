using Libro.Models;
using Xunit;

namespace Libro.Tests
{
    public class BookAvailabilityTests
    {
        [Fact]
        public void CanBeBorrowed_AvailableWithCopies_ReturnsTrue()
        {
            // Arrange
            var book = new Book
            {
                Title = "Test Book",
                Status = BookStatus.Available,
                AvailableCopies = 3
            };

            // Act
            bool result = book.CanBeBorrowed;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanBeBorrowed_AvailableWithZeroCopies_ReturnsFalse()
        {
            var book = new Book
            {
                Title = "Test Book",
                Status = BookStatus.Available,
                AvailableCopies = 0
            };

            bool result = book.CanBeBorrowed;

            Assert.False(result);
        }

        [Fact]
        public void CanBeBorrowed_ComingSoonWithCopies_ReturnsFalse()
        {
            // حتى لو فيه نسخ، طالما الـ Librarian حددها ComingSoon يدوي، تفضل مش متاحة
            var book = new Book
            {
                Title = "Test Book",
                Status = BookStatus.ComingSoon,
                AvailableCopies = 5
            };

            bool result = book.CanBeBorrowed;

            Assert.False(result);
        }

        [Fact]
        public void CanBeBorrowed_UnavailableStatus_ReturnsFalse()
        {
            var book = new Book
            {
                Title = "Test Book",
                Status = BookStatus.Unavailable,
                AvailableCopies = 10
            };

            bool result = book.CanBeBorrowed;

            Assert.False(result);
        }

        [Fact]
        public void DisplayStatusForMember_ComingSoon_ShowsComingSoonLabel()
        {
            var book = new Book
            {
                Title = "Test Book",
                Status = BookStatus.ComingSoon,
                AvailableCopies = 0
            };

            Assert.Equal("Coming Soon", book.DisplayStatusForMember);
        }

        [Fact]
        public void DisplayStatusForMember_AvailableWithCopies_ShowsAvailableLabel()
        {
            var book = new Book
            {
                Title = "Test Book",
                Status = BookStatus.Available,
                AvailableCopies = 2
            };

            Assert.Equal("Available", book.DisplayStatusForMember);
        }
    }
}