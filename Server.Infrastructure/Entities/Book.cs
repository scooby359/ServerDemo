using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.Infrastructure.Entities
{
    /// <summary>
    /// The book entity
    /// </summary>
    public class Book
    {
        /// <summary>
        /// Gets or sets the id
        /// </summary>
        [BsonId]
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the author
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Gets or sets the year
        /// </summary>
        public string Year { get; set; }
    }
}
