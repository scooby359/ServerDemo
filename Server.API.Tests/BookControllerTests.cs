using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Server.API.Controllers;
using Server.Domain.Models;
using Server.Domain.Services;

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
            var resultBody = JsonConvert.DeserializeObject<List<Controllers.Models.Book>>(result.Value);
            Assert.AreEqual(2, resultBody.Count);
            Assert.IsTrue(resultBody.Exists(x => x.Id == domainModels[0].Id));
            Assert.IsTrue(resultBody.Exists(x => x.Id == domainModels[1].Id));
        }

        #endregion

        #region Get by id
        #endregion

        #region Post
        #endregion

        #region Put
        #endregion

        #region Delete
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
        #endregion
    }
}
