using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using Server.Infrastructure.Entities;
using Server.Infrastructure.Repositories;
using Server.Infrastructure.Settings;

namespace Server.Infrastructure.Tests
{
    [TestFixture]
    public class RepositoryTests
    {
        private IRepository _repository;

        [SetUp]
        public void Run()
        {
            var settings = new BookRepositorySettings()
            {
                ConnectionString = "localhost:27017",
                Collection = "Books",
                Database = "library"
            };

            var mockSettings = new Mock<IOptions<BookRepositorySettings>>();
            mockSettings.Setup(x => x.Value).Returns(settings);

            _repository = new Repository(mockSettings.Object);

        }

        [Test]
        public async Task RepoTest()
        {
            // Setup test book
            var mockBookOne = new Book()
            {
                Id = Guid.NewGuid().ToString(),
                Author = "John Smith",
                Name = "Cool Book",
                Year = "2018"
            };

            // Act - Test insert
            await _repository.CreateBook(mockBookOne);

            // Act / Assert - Get Book
            var result = await _repository.GetBook(mockBookOne.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(mockBookOne.Id, result.Id);
            Assert.AreEqual(mockBookOne.Name, result.Name);
            Assert.AreEqual(mockBookOne.Author, result.Author);
            Assert.AreEqual(mockBookOne.Year, result.Year);

            //  Create second test entity
            var mockBookTwo = new Book()
            {
                Id = Guid.NewGuid().ToString(),
                Author = "Mel Smith",
                Name = "Funky Book",
                Year = "2017"
            };

            // Act - Test insert
            await _repository.CreateBook(mockBookTwo);

            // Act / Assert - Get Book
            result = await _repository.GetBook(mockBookTwo.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(mockBookTwo.Id, result.Id);
            Assert.AreEqual(mockBookTwo.Name, result.Name);
            Assert.AreEqual(mockBookTwo.Author, result.Author);
            Assert.AreEqual(mockBookTwo.Year, result.Year);
        
            // Act - test get all
            var listResult = await _repository.GetBooks();
            Assert.AreEqual(2, listResult.Count);
            Assert.IsTrue(listResult.Contains(mockBookOne));
            Assert.IsTrue(listResult.Contains(mockBookTwo));

            // Test delete
            await _repository.DeleteBook(mockBookOne.Id);
            await _repository.DeleteBook(mockBookTwo.Id);

            // Assert
            listResult = await _repository.GetBooks();
            Assert.AreEqual(0, listResult.Count);
        }
    }
}
