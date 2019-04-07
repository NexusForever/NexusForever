using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public enum BroadcastTier
    {
        High    = 0, // Notification Windows & Chat Message
        Medium  = 1, // Notification Message centre screen & Chat Message
        Low     = 2  // Chat Message
    }
}
