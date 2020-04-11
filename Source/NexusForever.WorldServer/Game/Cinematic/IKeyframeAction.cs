using NexusForever.WorldServer.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Cinematic
{
    public interface IKeyframeAction
    {
        void Send(WorldSession session);
    }
}
