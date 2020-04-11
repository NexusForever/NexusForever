using NexusForever.WorldServer.Game.Cinematic.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Cinematic
{
    public class VisualEffectEnd : IKeyframeAction
    {
        public uint Delay { get; }
        public uint VisualEffectId { get; }

        public VisualEffectEnd(uint delay, uint visualEffectId)
        {
            Delay = delay;
            VisualEffectId = visualEffectId;
        }

        public void Send(WorldSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicVisualEffectEnd
            {
                Delay = Delay,
                VisualHandle = VisualEffectId
            });
        }
    }
}
