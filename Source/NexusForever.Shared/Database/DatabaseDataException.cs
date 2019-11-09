using System;

namespace NexusForever.Shared.Database
{
    public class DatabaseDataException : Exception
    {
        public DatabaseDataException(string message)
            : base(message)
        {
        }
    }
}
