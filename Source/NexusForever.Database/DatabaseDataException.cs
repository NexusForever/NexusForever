using System;

namespace NexusForever.Database
{
    public class DatabaseDataException : Exception
    {
        public DatabaseDataException(string message)
            : base(message)
        {
        }
    }
}
