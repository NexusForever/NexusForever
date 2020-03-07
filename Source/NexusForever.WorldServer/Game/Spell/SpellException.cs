using System;

namespace NexusForever.WorldServer.Game.Spell
{
    public class SpellException : Exception
    {
        public SpellException(string message = "")
            : base(message)
        {
        }
    }
}
