using System;

namespace NexusForever.WorldServer.Game.Housing
{
    public class HousingException : Exception
    {
        public HousingException(string message = "")
            : base(message)
        {
        }
    }
}
