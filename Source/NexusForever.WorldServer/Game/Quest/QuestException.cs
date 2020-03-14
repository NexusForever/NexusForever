using System;

namespace NexusForever.WorldServer.Game.Quest
{
    public class QuestException : Exception
    {
        public QuestException(string message = "")
            : base(message)
        {
        }
    }
}
