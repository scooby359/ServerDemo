using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Server.Infrastructure.Entities;
using Server.Infrastructure.Settings;

namespace Server.Infrastructure.Repositories
{
    public class Repository: IRepository
    {
        private readonly IMongoCollection<Book> _bookCollection;

        public Repository(IOptions<BookRepositorySettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.Database);
            _bookCollection = database.GetCollection<Book>(settings.Value.Collection);
        }

        public async Task CreateBook(Book book)
        {
            await _bookCollection.InsertOneAsync(book);
        }

        public async Task<Book> GetBook(string id)
        {
            var filter = Builders<Book>.Filter.Eq(nameof(Book.Id), id);
            return await _bookCollection.Find(filter).FirstAsync();
        }

        public async Task<List<Book>> GetBooks()
        {
            return await _bookCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task UpdateBook(Book book)
        {
            var filter = Builders<Book>.Filter.Eq(nameof(Book.Id), book.Id);
            await _bookCollection.ReplaceOneAsync(filter, book);
        }

        public async Task DeleteBook(string id)
        {
            var filter = Builders<Book>.Filter.Eq(nameof(Book.Id), id);
            await _bookCollection.DeleteOneAsync(filter);
        }
    }
}
