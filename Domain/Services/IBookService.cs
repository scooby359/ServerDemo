using Server.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Server.Domain.Services
{
    /// <summary>
    /// The book service
    /// </summary>
    public interface IBookService
    {
        /// <summary>
        /// Creates a new book
        /// </summary>
        /// <param name="model">The book to create</param>
        /// <returns><see cref="Book"></see></returns>
        Task<Book> CreateBook(Book model);

        /// <summary>
        /// Gets a book by id
        /// </summary>
        /// <param name="id">The id to seearch for</param>
        /// <returns><see cref="Task{Book}"</returns>
        Task<Book> GetBook(string id);

        /// <summary>
        /// Gets all books
        /// </summary>
        /// <returns><see cref="List{Book}"</returns>
        Task<List<Book>> GetBooks();

        /// <summary>
        /// Updates an existing book
        /// </summary>
        /// <param name="model">The book to update</param>
        /// <returns><see cref="Task"</returns>
        Task UpdateBook(Book model);

        /// <summary>
        /// Delete a book
        /// </summary>
        /// <param name="id">The book id to delete</param>
        /// <returns></returns>
        Task DeleteBook(string id);
    }
}
