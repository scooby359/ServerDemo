using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Domain.Exceptions
{
    public class BookNotFoundException : Exception
    {
        public BookNotFoundException()
        {
        }

        public BookNotFoundException(string message) : base(message)
        {
        }
    }
}
