using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Server.Domain.Exceptions;
using Server.Domain.Models;
using Server.Domain.Services;
using Server.Infrastructure.Repositories;

namespace Server.Domain.Tests
{
    [TestFixture]
    public class BookServiceTests
    {
        #region CreateBook

        // Null param
        [Test]
        public void WhenCreateBook_AndParameterNull_ExceptionThrown()
        {
            var repo = new Mock<IRepository>();
            var service = new BookService(repo.Object);

            Assert.That(() => service.CreateBook(null), Throws.ArgumentNullException);
            repo.Verify(repository => repository.CreateBook(It.IsAny<Infrastructure.Entities.Book>()), Times.Never);

        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void WhenCreateBook_AndNameNullOrEmpty_ExceptionThrown(string name)
        {
            var repo = new Mock<IRepository>();
            var service = new BookService(repo.Object);
            var book = new Book
            {
                Year = "2019",
                Author = "Test Author",
                Name = name
            };

            Assert.That(() => service.CreateBook(book), Throws.ArgumentNullException);
            repo.Verify(repository => repository.CreateBook(It.IsAny<Infrastructure.Entities.Book>()), Times.Never);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void WhenCreateBook_AndYearNullOrEmpty_ExceptionThrown(string year)
        {
            var repo = new Mock<IRepository>();
            var service = new BookService(repo.Object);
            var book = new Book
            {
                Year = year,
                Author = "Test Author",
                Name = "Book"
            };

            Assert.That(() => service.CreateBook(book), Throws.ArgumentNullException);
            repo.Verify(repository => repository.CreateBook(It.IsAny<Infrastructure.Entities.Book>()), Times.Never);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void WhenCreateBook_AndAuthorNullOrEmpty_ExceptionThrown(string author)
        {
            var repo = new Mock<IRepository>();
            var service = new BookService(repo.Object);
            var book = new Book
            {
                Year = "2019",
                Author = author,
                Name = "Book"
            };

            Assert.That(() => service.CreateBook(book), Throws.ArgumentNullException);
            repo.Verify(repository => repository.CreateBook(It.IsAny<Infrastructure.Entities.Book>()), Times.Never);
        }

        // Db fails
        [Test]
        public async Task WhenCreateBook_AndRepoThrowsException_ExceptionThrown()
        {
            var repo = new Mock<IRepository>();
            repo.Setup(repository => repository.CreateBook(It.IsAny<Infrastructure.Entities.Book>()))
                .ThrowsAsync(new Exception());

            var service = new BookService(repo.Object);
            var book = new Book
            {
                Year = "2019",
                Author = "Test Author",
                Name = "Book"
            };

            try
            {
                var result = await service.CreateBook(book);
                Assert.Fail("Exception not thrown");
            }
            catch (Exception)
            {
                // This is a pass
            }

            repo.Verify(repository => repository.CreateBook(It.IsAny<Infrastructure.Entities.Book>()), Times.Once);
        }

        // Good create
        [Test]
        public async Task whenCreateBook_AndRequestValid_RepoIsCalledToInsertAndBookReturned()
        {
            // Arrange
            var repo = new Mock<IRepository>();
            var insertedBook = new Infrastructure.Entities.Book();
            repo.Setup(repository => repository.CreateBook(It.IsAny<Infrastructure.Entities.Book>()))
                .Callback((Infrastructure.Entities.Book repoBook) => insertedBook = repoBook)
                .Returns(Task.CompletedTask);

            var book = new Book
            {
                Year = "2019",
                Author = "Test Author",
                Name = "Book"
            };

            var service = new BookService(repo.Object);

            // Act
            var result = await service.CreateBook(book);

            // Assert
            Assert.IsTrue(result.Id == insertedBook.Id);
            Assert.IsTrue(result.Name == insertedBook.Name);
            Assert.IsTrue(result.Year == insertedBook.Year);
            Assert.IsTrue(result.Author == insertedBook.Author);

            Assert.IsTrue(result.Name == book.Name);
            Assert.IsTrue(result.Year == book.Year);
            Assert.IsTrue(result.Author == book.Author);

            repo.Verify(repository => repository.CreateBook(It.IsAny<Infrastructure.Entities.Book>()), Times.Once);
        }

        #endregion

        #region DeleteBook

        // null id
        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void WhenDeleteCalled_AndIDNullOrWhiteSpace_ExceptionThrown(string id)
        {
            var repo = new Mock<IRepository>();
            var service = new BookService(repo.Object);

            Assert.That(() => service.DeleteBook(id), Throws.ArgumentNullException);
            repo.Verify(repository => repository.DeleteBook(It.IsAny<string>()), Times.Never);
        }

        // book not found
        [Test]
        public async Task WhenDeleteCalled_AndBookNotFound_NotFoundExceptionThron()
        {
            // Arrange
            var repo = new Mock<IRepository>();
            var service = new BookService(repo.Object);
            var requestedId = string.Empty;
            repo.Setup(repository => repository.GetBook(It.IsAny<string>()))
                .Callback((string id) => requestedId = id)
                .Returns(Task.FromResult<Infrastructure.Entities.Book>(null));
            var bookId = Guid.NewGuid().ToString();

            // Act
            try
            {
                await service.DeleteBook(bookId);
                Assert.Fail("Exception not thrown");
            }
            catch (BookNotFoundException)
            {
                // This is a pass
            }
            catch (Exception)
            {
                Assert.Fail("Wrong exception thrown");
            }

            // Assert
            Assert.IsTrue(bookId == requestedId);
            repo.Verify(repository => repository.GetBook(It.IsAny<string>()), Times.Once);
            repo.Verify(repository => repository.DeleteBook(It.IsAny<string>()), Times.Never);
        }

        // book deleted
        [Test]
        public async Task WhenDeleteCalled_AndIDFound_RepoIsCalledToDeleteBook()
        {
            // Arrange
            var repo = new Mock<IRepository>();
            var service = new BookService(repo.Object);
            var getId = string.Empty;
            var deleteId = string.Empty;
            var bookId = Guid.NewGuid().ToString();
            var mockBook = new Infrastructure.Entities.Book
                {Year = "2019", Id = bookId, Author = "Chris", Name = "Book"};

            repo.Setup(repository => repository.GetBook(It.IsAny<string>()))
                .Callback((string id) => getId = id)
                .Returns(Task.FromResult(mockBook));

            repo.Setup(repository => repository.DeleteBook(It.IsAny<string>()))
                .Callback((string id) => deleteId = id)
                .Returns(Task.CompletedTask);

            // Act

            await service.DeleteBook(bookId);

            // Assert

            Assert.IsTrue(bookId == getId, "GetID does not match BookID");
            Assert.IsTrue(bookId == deleteId, "DeleteID does not match BookID");
            repo.Verify(repository => repository.GetBook(It.IsAny<string>()), Times.Once);
            repo.Verify(repository => repository.DeleteBook(It.IsAny<string>()), Times.Once);
        }

        #endregion
    }
}
