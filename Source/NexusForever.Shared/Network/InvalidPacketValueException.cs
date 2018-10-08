using System;

namespace NexusForever.Shared.Network
{
    public class InvalidPacketValueException : Exception
    {
        public InvalidPacketValueException()
        {
        }

        public InvalidPacketValueException(string message)
            : base(message)
        {
        }
    }
}
