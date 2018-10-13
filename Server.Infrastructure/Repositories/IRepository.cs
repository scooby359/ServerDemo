using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Server.Infrastructure.Entities;

namespace Server.Infrastructure.Repositories
{
    /// <summary>
    /// The Book repository
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Creates a book
        /// </summary>
        /// <param name="book">Book to create</param>
        /// <returns><see cref="Task"/></returns>
        Task CreateBook(Book book);
        
        /// <summary>
        /// Gets a single book
        /// </summary>
        /// <param name="id">The id to retrieve</param>
        /// <returns><see cref="Task{TResult}"/></returns>
        Task<Book> GetBook(string id);

        /// <summary>
        /// Gets all books
        /// </summary>
        /// <returns><see cref="Task{TResult}"/></returns>
        Task<List<Book>> GetBooks();

        /// <summary>
        /// Updates a book
        /// </summary>
        /// <param name="book">The book to update</param>
        /// <returns><see cref="Task"/></returns>
        Task UpdateBook(Book book);

        /// <summary>
        /// Deletes a book
        /// </summary>
        /// <param name="id">The id of the book to delete</param>
        /// <returns><see cref="Task"/></returns>
        Task DeleteBook(string id);
    }
}
