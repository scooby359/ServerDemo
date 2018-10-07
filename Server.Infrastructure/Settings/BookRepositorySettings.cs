using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Infrastructure.Settings
{
    public class BookRepositorySettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public string Collection { get; set; }
    }
}
