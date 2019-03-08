using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Domain.Exceptions;
using Server.Domain.Models;
using Server.Infrastructure.Repositories;

namespace Server.Domain.Services
{
    public class BookService : IBookService
    {
        private readonly IRepository _repository;

        public BookService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<Book> CreateBook(Book model)
        {
            // Check for empty param
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Check for missing values
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(model.Author)) 
                errors.Add(nameof(model.Author));
            if (string.IsNullOrWhiteSpace(model.Name))
                errors.Add(nameof(model.Name));
            if (string.IsNullOrWhiteSpace(model.Year))
                errors.Add(nameof(model.Year));
            if (errors.Any())
            {
                throw new ArgumentNullException(errors.ToString());
            }

            var repoBook = MapBookToRepoBook(model);
            repoBook.Id = Guid.NewGuid().ToString();

            try
            {
                await this._repository.CreateBook(repoBook);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("DB write failed", e);
            }

            return MapRepoBookToBook(repoBook);
        }

        public async Task DeleteBook(string id)
        {
            // Check for null id
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            // Check book exists in DB
            var book = await _repository.GetBook(id);

            if (book == null)
                throw new BookNotFoundException(id);

            // Delete book
            await _repository.DeleteBook(id);
        }

        public Task<Book> GetBook(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Book>> GetBooks()
        {
            throw new NotImplementedException();
        }

        public Task UpdateBook(Book model)
        {
            throw new NotImplementedException();
        }

        private Infrastructure.Entities.Book MapBookToRepoBook(Book model)
        {
            return new Infrastructure.Entities.Book
            {
                Id = model.Id,
                Year = model.Year,
                Author = model.Author,
                Name = model.Name
            };
        }

        private Book MapRepoBookToBook(Infrastructure.Entities.Book repoBook)
        {
            return new Book
            {
                Year = repoBook.Year,
                Author = repoBook.Author,
                Name = repoBook.Name,
                Id = repoBook.Id
            };
        }
    }
}
