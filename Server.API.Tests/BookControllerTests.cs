using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Server.API.Controllers;
using Server.API.Controllers.Models;
using Server.Domain.Exceptions;
using Server.Domain.Services;
using Book = Server.Domain.Models.Book;

namespace Server.API.Tests
{
    [TestFixture]
    public class BookControllerTests
    {
        #region Get all

        [Test]
        public async Task WhenGetAll_AndNoneFound_NoneReturned()
        {
            // Arrange
            var service = new Mock<IBookService>();
            var controller = new BookController(service.Object);

            // Act
            var result = await controller.Get() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            service.Verify(x => x.GetBooks(), Times.Once);
        }

        [Test]
        public async Task WhenGetAll_AndBooksFound_BooksReturned()
        {
            // Arrange
            var domainModels = new List<Book> {CreateDomainBook(), CreateDomainBook()};

            var service = new Mock<IBookService>();
            service.Setup(x => x.GetBooks()).Returns(Task.FromResult(domainModels));
            var controller = new BookController(service.Object);

            // Act
            var result = await controller.Get() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var resultBody = result.Value as List<Controllers.Models.Book>;
            Assert.AreEqual(2, resultBody.Count);
            Assert.IsTrue(resultBody.Exists(x => x.Id == domainModels[0].Id));
            Assert.IsTrue(resultBody.Exists(x => x.Id == domainModels[1].Id));
            service.Verify(x => x.GetBooks(), Times.Once);
        }

        #endregion

        #region Get by id

        [Test]
        public async Task WhenGetById_AndBookFound_BookReturned()
        {
            // Arrange
            var book = CreateDomainBook();
            var service = new Mock<IBookService>();
            service.Setup(x => x.GetBook(It.IsAny<string>())).Returns(Task.FromResult(book));

            var controller = new BookController(service.Object);

            // Act
            var result = await controller.Get(Guid.NewGuid().ToString()) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var resultBody = result.Value as Controllers.Models.Book;
            Assert.AreEqual(book.Id, resultBody.Id);
            Assert.AreEqual(book.Year, resultBody.Year);
            Assert.AreEqual(book.Name, resultBody.Name);
            Assert.AreEqual(book.Author, resultBody.Author);
            service.Verify(x => x.GetBook(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task WhenGetById_AndBookNotFound_NotFoundReturned()
        {
            // Arrange
            var service = new Mock<IBookService>();
            service.Setup(x => x.GetBook(It.IsAny<string>())).ThrowsAsync(new BookNotFoundException());

            var controller = new BookController(service.Object);

            // Act
            var result = await controller.Get(Guid.NewGuid().ToString()) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            service.Verify(x => x.GetBook(It.IsAny<string>()), Times.Once);

        }

        [Test]
        public async Task WhenGetById_AndExceptionThrown_InternalServerErrorReturned()
        {
            // Arrange
            var service = new Mock<IBookService>();
            service.Setup(x => x.GetBook(It.IsAny<string>())).ThrowsAsync(new Exception());

            var controller = new BookController(service.Object);

            // Act
            var result = await controller.Get(Guid.NewGuid().ToString()) as ObjectResult;

            // Assert
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, result.StatusCode);
            service.Verify(x => x.GetBook(It.IsAny<string>()), Times.Once);

        }

        #endregion

        #region Post

        [Test]
        public async Task WhenPost_AndParametersHaveWhiteSpace_ThenBadRequest()
        {
            // Arrange
            var request = new Create {Author = "Me", Name = "Book"};
            var service = new Mock<IBookService>();
            service.Setup(x => x.CreateBook(It.IsAny<Book>())).ThrowsAsync(new ArgumentNullException());
            var controller = new BookController(service.Object);

            // Act
            var result = await controller.Post(request) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            service.Verify(x => x.CreateBook(It.IsAny<Book>()), Times.Once);
        }

        [Test]
        public async Task WhenPost_AndServiceThrowsException_ThenInternalErrorReturned()
        {
            // Arrange
            var request = new Create { Author = "Me", Name = "Book" };
            var service = new Mock<IBookService>();
            service.Setup(x => x.CreateBook(It.IsAny<Book>())).ThrowsAsync(new Exception());
            var controller = new BookController(service.Object);

            // Act
            var result = await controller.Post(request) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, result.StatusCode);
            service.Verify(x => x.CreateBook(It.IsAny<Book>()), Times.Once);
        }

        [Test]
        public async Task WhenPost_AndModelIsValid_ThenOkObjectReturned()
        {
            // Arrange
            var request = new Create { Author = "Me", Name = "Book", Year = "2018"};
            var book = new Book
                {Id = Guid.NewGuid().ToString(), Author = request.Author, Name = request.Name, Year = request.Year};
            var bookArg = new Book();

            var service = new Mock<IBookService>();
            service.Setup(x => x.CreateBook(It.IsAny<Book>()))
                .Callback((Book arg) =>
                {
                    bookArg.Author = arg.Author;
                    bookArg.Name = arg.Name;
                    bookArg.Year = arg.Year;
                })
                .Returns(Task.FromResult(book));
            var controller = new BookController(service.Object);

            // Act
            var result = await controller.Post(request) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var resultBody = result.Value as Controllers.Models.Book;
            Assert.AreEqual(book.Id, resultBody.Id);
            Assert.AreEqual(book.Name, resultBody.Name);
            Assert.AreEqual(book.Author, resultBody.Author);
            Assert.AreEqual(book.Year, resultBody.Year);

            Assert.AreEqual(request.Name, bookArg.Name);
            Assert.AreEqual(request.Year, bookArg.Year);
            Assert.AreEqual(request.Author, bookArg.Author);

            service.Verify(x => x.CreateBook(It.IsAny<Book>()), Times.Once);
        }

        #endregion

        #region Put

        [Test]
        public async Task WhenPut_AndRequestOk_ThenOkReturned()
        {
            // Arrange
            var book = CreateApiBook();
            var service = new Mock<IBookService>();
            service.Setup(x => x.UpdateBook(It.IsAny<Book>())).Returns(Task.CompletedTask);

            var controller = new BookController(service.Object);

            // Act
            var result = await controller.Put(book) as OkResult;

            // Assert
            Assert.IsNotNull(result);
            service.Verify(x=>x.UpdateBook(It.IsAny<Book>()), Times.Once);
        }

        [Test]
        public async Task WhenPut_AndArgumentsNull_BadRequestReturned()
        {
            // Arrange
            var book = CreateApiBook();
            var service = new Mock<IBookService>();
            service.Setup(x => x.UpdateBook(It.IsAny<Book>())).ThrowsAsync(new ArgumentNullException());

            var controller = new BookController(service.Object);

            // Act
            var result = await controller.Put(book) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            service.Verify(x => x.UpdateBook(It.IsAny<Book>()), Times.Once);
        }

        [Test]
        public async Task WhenPut_AndBookNotFound_NotFoundReturned()
        {
            // Arrange
            var book = CreateApiBook();
            var service = new Mock<IBookService>();
            service.Setup(x => x.UpdateBook(It.IsAny<Book>())).ThrowsAsync(new BookNotFoundException());

            var controller = new BookController(service.Object);

            // Act
            var result = await controller.Put(book) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            service.Verify(x => x.UpdateBook(It.IsAny<Book>()), Times.Once);
        }

        [Test]
        public async Task WhenPut_AndServiceThrowsError_InternalServerErrorReturned()
        {
            // Arrange
            var book = CreateApiBook();
            var service = new Mock<IBookService>();
            service.Setup(x => x.UpdateBook(It.IsAny<Book>())).ThrowsAsync(new Exception());

            var controller = new BookController(service.Object);

            // Act
            var result = await controller.Put(book) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, result.StatusCode);
            service.Verify(x => x.UpdateBook(It.IsAny<Book>()), Times.Once);
        }
        #endregion

        #region Delete

        [Test]
        public async Task WhenDelete_AndRequestValid_OkReturned()
        {
            // Arrange
            var service = new Mock<IBookService>();
            service.Setup(x => x.DeleteBook(It.IsAny<string>())).Returns(Task.CompletedTask);

            var controller = new BookController(service.Object);

            // Act
            var result = await controller.Delete(Guid.NewGuid().ToString()) as OkResult;

            // Assert
            Assert.IsNotNull(result);
            service.Verify(x => x.DeleteBook(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task WhenDelete_AndBookNotFound_NotFoundReturned()
        {
            // Arrange
            var service = new Mock<IBookService>();
            service.Setup(x => x.DeleteBook(It.IsAny<string>())).ThrowsAsync(new BookNotFoundException());

            var controller = new BookController(service.Object);

            // Act
            var result = await controller.Delete(Guid.NewGuid().ToString()) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            service.Verify(x => x.DeleteBook(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task WhenDelete_AndServiceThrowsException_InternalSeverErrorReturned()
        {
            // Arrange
            var service = new Mock<IBookService>();
            service.Setup(x => x.DeleteBook(It.IsAny<string>())).ThrowsAsync(new Exception());

            var controller = new BookController(service.Object);

            // Act
            var result = await controller.Delete(Guid.NewGuid().ToString()) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, result.StatusCode);
            service.Verify(x => x.DeleteBook(It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region helpers

        private static Book CreateDomainBook()
        {
            return new Book()
            {
                Id = Guid.NewGuid().ToString(),
                Author = "Author",
                Name = "Name",
                Year = "2018"
            };
        }

        private static Controllers.Models.Book CreateApiBook()
        {
            return new Controllers.Models.Book()
            {
                Id = Guid.NewGuid().ToString(),
                Author = "Author",
                Name = "Name",
                Year = "2018"
            };
        }
        #endregion
    }
}
