using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.API.Controllers.Models
{
    public class Book
    {
        /// <summary>
        /// Gets or sets the id
        /// </summary>
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
