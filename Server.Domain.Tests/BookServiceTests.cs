using System;
using System.Collections.Generic;
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

        #region Create
        [Test]
        public void WhenCreateBook_AndArgumentNull_ThenExceptionThrown()
        {
            // Arrange
            var repo = new Mock<IRepository>();
            var service = new BookService(repo.Object);

            // Act / Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await service.CreateBook(null));
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
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.CreateBook(book));
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
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.CreateBook(book));
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
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.CreateBook(book));
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
            repo.Verify(x => x.CreateBook(It.IsAny<Infrastructure.Entities.Book>()), Times.Once, "Repo not called, or called more than once");
        }

        #endregion

        #region Delete
        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void WhenDelete_AndIdNullOrWhiteSpace_ThenExceptionThrown(string id)
        {
            // Arrange
            var repo = new Mock<IRepository>();
            var service = new BookService(repo.Object);

            // Act / Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.DeleteBook(id));
        }

        [Test]
        public void WhenDelete_AndRecordNotFound_ThenExceptionThrown()
        {
            // Arrange
            var repo = new Mock<IRepository>();
            repo.Setup(x => x.GetBook(It.IsAny<string>())).Returns(Task.FromResult<Infrastructure.Entities.Book>(null));

            var service = new BookService(repo.Object);

            // Act / Assert
            Assert.ThrowsAsync<BookNotFoundException>(async () => await service.DeleteBook(Guid.NewGuid().ToString()));
            repo.Verify(x => x.GetBook(It.IsAny<string>()), Times.Once, "Service was not called, or called more than once");
            repo.Verify(x => x.DeleteBook(It.IsAny<string>()), Times.Never, "Service was called");
        }

        [Test]
        public async Task WhenDelete_AndRecordFound_ThenRepoCalled()
        {
            // Arrange
            var entity = new Infrastructure.Entities.Book
            {
                Id = Guid.NewGuid().ToString(),
                Author = "Author",
                Name = "Name",
                Year = "2018"
            };
            var mockId = string.Empty;

            var repo = new Mock<IRepository>();
            repo.Setup(x => x.GetBook(It.IsAny<string>()))
                .Callback((string id) => mockId = id)
                .Returns(Task.FromResult(entity));
            repo.Setup(x => x.DeleteBook(It.IsAny<string>())).Returns(Task.CompletedTask);

            var service = new BookService(repo.Object);

            // Act
            await service.DeleteBook(entity.Id);

            // Assert
            Assert.AreEqual(entity.Id, mockId);
            repo.Verify(x => x.GetBook(It.IsAny<string>()), Times.Once, "Service was not called or called more than once");
            repo.Verify(x => x.DeleteBook(It.IsAny<string>()), Times.Once, "Service was not called or called more than once");
        }
        #endregion

        #region Get by ID

        [Test]
        [TestCase("")]
        [TestCase("     ")]
        [TestCase(null)]
        public void WhenGetById_AndIdNullOrWhiteSpace_ExceptionThrown(string id)
        {
            // Arrange
            var repo = new Mock<IRepository>();
            var service = new BookService(repo.Object);

            // Act / Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.GetBook(id));
        }

        [Test]
        public void WhenGetById_AndBookNotFound_ExceptionThrown()
        {
            // Arrange
            var repo = new Mock<IRepository>();
            repo.Setup(x => x.GetBook(It.IsAny<string>())).Returns(Task.FromResult<Infrastructure.Entities.Book>(null));
            var service = new BookService(repo.Object);
            

            // Act / Assert
            Assert.ThrowsAsync<BookNotFoundException>(async () => await service.GetBook(Guid.NewGuid().ToString()));
            repo.Verify(x => x.GetBook(It.IsAny<string>()), Times.Once, "Service was not called or called more than once");
        }

        [Test]
        public async Task WhenGetById_AndBookFound_BookReturned()
        {
            // Arrange
            var entity = CreateBookEntity();
            var repo = new Mock<IRepository>();
            repo.Setup(x => x.GetBook(It.IsAny<string>())).Returns(Task.FromResult(entity));
            var service = new BookService(repo.Object);


            // Act 
            var result = await service.GetBook(Guid.NewGuid().ToString()) as Book;
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(entity.Id, result.Id);
            Assert.AreEqual(entity.Author, result.Author);
            Assert.AreEqual(entity.Name, result.Name);
            Assert.AreEqual(entity.Year, result.Year);
            repo.Verify(x => x.GetBook(It.IsAny<string>()), Times.Once, "Service was not called or called more than once");
        }

        #endregion

        #region Get all

        [Test]
        public async Task WhenGetAll_AndNoneFound_NoneReturned()
        {
            // Arrange
            var repo = new Mock<IRepository>();
            repo.Setup(x => x.GetBooks()).Returns(Task.FromResult<List<Infrastructure.Entities.Book>>(null));
            var service = new BookService(repo.Object);

            // Act
            var result = await service.GetBooks() as List<Book>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
            repo.Verify(x => x.GetBooks(), Times.Once, "Repo was not called or called more than once");
        }

        [Test]
        public async Task WhenGetAll_AndResultsFound_ResultsReturned()
        {
            // Arrange
            var entityList = new List<Infrastructure.Entities.Book>();
            entityList.Add(CreateBookEntity());
            entityList.Add(CreateBookEntity());

            var repo = new Mock<IRepository>();
            repo.Setup(x => x.GetBooks()).Returns(Task.FromResult(entityList));
            var service = new BookService(repo.Object);

            // Act
            var result = await service.GetBooks() as List<Book>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Exists(x => x.Id == entityList[0].Id));
            Assert.IsTrue(result.Exists(x => x.Id == entityList[1].Id));
            repo.Verify(x => x.GetBooks(), Times.Once, "Repo was not called or called more than once");
        }

        #endregion

        #region Update

        [Test]
        public void WhenUpdate_IfArgumentNull_ExceptionThrown()
        {
            // Arrange
            var repo = new Mock<IRepository>();
            var service = new BookService(repo.Object);

            // Act // Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await service.UpdateBook(null));
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void WhenUpdate_IfIdNullOrWhitespace_ExceptionThrown(string id)
        {
            // Arrange
            var book = CreateBook();
            book.Id = id;
            var repo = new Mock<IRepository>();
            var service = new BookService(repo.Object);

            // Act // Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateBook(book));
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void WhenUpdate_IfAuthorNullOrWhitespace_ExceptionThrown(string author)
        {
            // Arrange
            var book = CreateBook();
            book.Author = author;
            var repo = new Mock<IRepository>();
            var service = new BookService(repo.Object);

            // Act // Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateBook(book));
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void WhenUpdate_IfNameNullOrWhitespace_ExceptionThrown(string name)
        {
            // Arrange
            var book = CreateBook();
            book.Name = name;
            var repo = new Mock<IRepository>();
            var service = new BookService(repo.Object);

            // Act // Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateBook(book));
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void WhenUpdate_IfYearNullOrWhitespace_ExceptionThrown(string year)
        {
            // Arrange
            var book = CreateBook();
            book.Year = year;
            var repo = new Mock<IRepository>();
            var service = new BookService(repo.Object);

            // Act // Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateBook(book));
        }

        [Test]
        public void WhenUpdate_IfBookNotFound_ExceptionThrown()
        {
            // Arrange
            var book = CreateBook();
            var repo = new Mock<IRepository>();
            repo.Setup(x => x.GetBook(It.IsAny<string>())).Returns(Task.FromResult<Infrastructure.Entities.Book>(null));
            var service = new BookService(repo.Object);

            // Act // Assert
            Assert.ThrowsAsync<BookNotFoundException>(async () => await service.UpdateBook(book));
            repo.Verify(x => x.GetBook(It.IsAny<string>()),Times.Once, "Repo was not called or called more than once");
            repo.Verify(x => x.UpdateBook(It.IsAny<Infrastructure.Entities.Book>()), Times.Never, "Repo was called");
        }

        [Test]
        public async Task WhenUpdate_AndRequestOk_ThenRepoCalled()
        {
            // Arrange
            var book = CreateBook();
            var entity = new Infrastructure.Entities.Book()
            {
                Id = book.Id,
                Year = book.Year,
                Author = book.Author,
                Name = book.Name
            };

            var entityArg = new Infrastructure.Entities.Book();
            var repo = new Mock<IRepository>();
            repo.Setup(x => x.GetBook(It.IsAny<string>())).Returns(Task.FromResult(entity));
            repo.Setup(x => x.UpdateBook(It.IsAny<Infrastructure.Entities.Book>()))
                .Callback((Infrastructure.Entities.Book updateEntity) =>
                {
                    entityArg.Id = updateEntity.Id;
                    entityArg.Name = updateEntity.Name;
                    entityArg.Year = updateEntity.Year;
                    entityArg.Author = updateEntity.Author;
                })
                .Returns(Task.CompletedTask);

            var service = new BookService(repo.Object);

            // Act
            await service.UpdateBook(book);

            // Assert
            repo.Verify(x => x.GetBook(It.IsAny<string>()), Times.Once, "Repo was not called or called more than once");
            repo.Verify(x => x.UpdateBook(It.IsAny<Infrastructure.Entities.Book>()), Times.Once, "Repo was not called or called more than once");
            Assert.AreEqual(book.Id, entityArg.Id);
            Assert.AreEqual(book.Name, entityArg.Name);
            Assert.AreEqual(book.Year, entityArg.Year);
            Assert.AreEqual(book.Author, entityArg.Author);
        }

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

        private Infrastructure.Entities.Book CreateBookEntity()
        {
            return new Infrastructure.Entities.Book()
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
