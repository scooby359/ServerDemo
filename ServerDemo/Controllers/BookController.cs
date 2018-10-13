using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.API.Controllers.Models;
using Server.Domain.Exceptions;
using Server.Domain.Services;

namespace Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Gets all books
        /// </summary>
        /// <returns><see cref="List{Book}"/></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var models = await _bookService.GetBooks();

            var result = new List<Book>();

            if (models == null || models.Count == 0)
                return new OkObjectResult(result);

            foreach (var model in models)
            {
                result.Add(MapToModel(model));
            }

            return new OkObjectResult(result);

        }
        
        /// <summary>
        /// Get a single book by ID
        /// </summary>
        /// <param name="id">The book to return</param>
        /// <returns><see cref="Book"/></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var model = await _bookService.GetBook(id);
                var result = MapToModel(model);
                return new OkObjectResult(result);
            }
            catch (BookNotFoundException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e);
            }

        }

        /// <summary>
        /// Creates a new book
        /// </summary>
        /// <param name="request">The new request</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Create request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var createRequest = new Domain.Models.Book
                {
                    Author = request.Author,
                    Name = request.Name,
                    Year = request.Year
                };

                var model = await _bookService.CreateBook(createRequest);
                var result = MapToModel(model);

                return new OkObjectResult(result);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e);
            }
            catch (Exception e)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, e);
            }
        }

        /// <summary>
        /// Update an existing book
        /// </summary>
        /// <param name="book">The update request</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var model = MapToDomain(book);
                await _bookService.UpdateBook(model);
                return Ok();
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e);
            }
            catch (BookNotFoundException e)
            {
                return NotFound(e);
            }
            catch (Exception e)
            {
             return StatusCode((int)HttpStatusCode.InternalServerError, e);
            }
        }

        /// <summary>
        /// Deletes a book
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _bookService.DeleteBook(id);
                return Ok();
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e);
            }
            catch (BookNotFoundException e)
            {
                return NotFound(e);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e);
            }
        }

        #region helpers

        private static Book MapToModel(Domain.Models.Book book)
        {
            return new Book()
            {
                Id = book.Id,
                Year = book.Year,
                Author = book.Author,
                Name = book.Name
            };
        }

        private static Domain.Models.Book MapToDomain(Book book)
        {
            return new Domain.Models.Book()
            {
                Id = book.Id,
                Year = book.Year,
                Author = book.Author,
                Name = book.Name
            };
        }

        #endregion
    }
}
