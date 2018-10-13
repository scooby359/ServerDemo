using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Server.Domain.Exceptions;
using Server.Domain.Models;
using Server.Infrastructure.Repositories;
using Entity = Server.Infrastructure.Entities;

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
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(model.Name))
                errors.Add(nameof(model.Name));

            if (string.IsNullOrWhiteSpace(model.Author))
                errors.Add(nameof(model.Author));

            if (string.IsNullOrWhiteSpace(model.Year))
                errors.Add(nameof(model.Name));

            if (errors.Count > 0)
                throw new ArgumentNullException(errors.ToString());

            model.Id = Guid.NewGuid().ToString();

            var entity = MapModelToEntity(model);

            await _repository.CreateBook(entity);

            return model;
        }

        public async Task DeleteBook(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            var entity = await _repository.GetBook(id);
            if (entity == null)
                throw new BookNotFoundException(id);

            await _repository.DeleteBook(id);
        }

        public async Task<Book> GetBook(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            var entity = await _repository.GetBook(id);
            if (entity == null)
                throw new BookNotFoundException(id);

            return MapEntityToModel(entity);
        }

        public async Task<List<Book>> GetBooks()
        {
            var entities = await _repository.GetBooks();

            var models = new List<Book>();

            if (entities == null || entities.Count == 0)
                return models;

            foreach (var book in entities)
            {
                models.Add(MapEntityToModel(book));
            }

            return models;
        }

        public async Task UpdateBook(Book model)
        {
         
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(model.Author))
                errors.Add(nameof(model.Author));
            if (string.IsNullOrWhiteSpace(model.Id))
                errors.Add(nameof(model.Id));
            if (string.IsNullOrWhiteSpace(model.Name))
                errors.Add(nameof(model.Name));
            if (string.IsNullOrWhiteSpace(model.Year))
                errors.Add(nameof(model.Year));

            if (errors.Count > 0)
                throw new ArgumentNullException(errors.ToString());

            var entity = await _repository.GetBook(model.Id);
            if (entity == null)
                throw new BookNotFoundException(model.Id);

            entity.Name = model.Name;
            entity.Author = model.Author;
            entity.Year = model.Year;

            await _repository.UpdateBook(entity);
        }


        #region helpers

        private Book MapEntityToModel(Entity.Book entity)
        {
            return new Book
            {
                Author = entity.Author,
                Id = entity.Id,
                Name = entity.Name,
                Year = entity.Year
            };
        }

        private Entity.Book MapModelToEntity(Book model)
        {
            return new Entity.Book
            {
                Author = model.Author,
                Id = model.Id,
                Name = model.Name,
                Year = model.Year
            };
        }
        #endregion
    }
}
