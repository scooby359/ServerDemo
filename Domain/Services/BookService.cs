using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Server.Domain.Models;

namespace Server.Domain.Services
{
    public class BookService : IBookService
    {
        public Task<Book> CreateBook(Book model)
        {
            throw new NotImplementedException();
        }

        public Task DeleteBook(string id)
        {
            throw new NotImplementedException();
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
    }
}
