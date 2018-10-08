using System;

namespace NexusForever.Shared.GameTable
{
    public class GameTableException : Exception
    {
        public GameTableException(string message)
            : base(message)
        {
        }
    }
}
