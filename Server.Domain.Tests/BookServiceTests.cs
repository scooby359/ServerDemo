using System;
using System.Threading.Tasks;
using Moq;
using NUnit;
using NUnit.Framework;
using Server.Domain.Models;
using Server.Domain.Services;
using Server.Infrastructure.Repositories;

namespace Server.Domain.Tests
{
    [TestFixture]
    public class BookServiceTests
    {

        #region Create
        [Test]
        public async Task WhenCreateBook_AndArgumentNull_ThenExceptionThrown()
        {
            // Arrange
            var repo = new Mock<IRepository>();
            var service = new BookService(repo.Object);

            // Act / Assert
            Assert.Throws<ArgumentNullException>(async () => await service.CreateBook(new Book()));
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void WhenCreateBook_AndNameInvalid_ThenExceptionThrown(string name)
        {
            // Arrange
            var book = CreateBook();
            book.Name = name;
            var repo = new Mock<IRepository>();
            var service = new BookService(repo.Object);

            // Act / Assert
            Assert.Throws<ArgumentNullException>(async () => await service.CreateBook(book));
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void WhenCreateBook_AndYearInvalid_ThenExceptionThrown(string year)
        {
            // Arrange
            var book = CreateBook();
            book.Year = year;
            var repo = new Mock<IRepository>();
            var service = new BookService(repo.Object);

            // Act / Assert
            Assert.Throws<ArgumentNullException>(async () => await service.CreateBook(book));
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void WhenCreateBook_AndAuthorInvalid_ThenExceptionThrown(string author)
        {
            // Arrange
            var book = CreateBook();
            book.Author = author;
            var repo = new Mock<IRepository>();
            var service = new BookService(repo.Object);

            // Act / Assert
            Assert.Throws<ArgumentNullException>(async () => await service.CreateBook(book));
        }

        [Test]
        public async Task WhenCreateBook_AndRequestValid_ThenRepoCalled()
        {
            // Arrange
            var book = CreateBook();
            var repo = new Mock<IRepository>();
            repo.Setup(x => x.CreateBook(It.IsAny<Infrastructure.Entities.Book>())).Returns(Task.CompletedTask);
            var service = new BookService(repo.Object);

            // Act 

            var result = await service.CreateBook(book);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(book, result);
        }

        #endregion

        #region Delete
        #endregion

        #region Get by ID
        #endregion

        #region Get all
        #endregion

        #region Update
        #endregion

        #region helpers

        private Book CreateBook()
        {
            return new Book
            {
                Author = "Author",
                Name = "Name",
                Year = "2018",
                Id = Guid.NewGuid().ToString()
            };
        }

        #endregion
    }
}
