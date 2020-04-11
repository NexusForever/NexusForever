using NexusForever.WorldServer.Game.Cinematic.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Cinematic
{
    public class Transition : IKeyframeAction
    {
        public uint Delay { get; }
        public uint Flags { get; }
        public uint EndTransition { get; }
        public ushort Start { get; }
        public ushort Mid { get; }
        public ushort End { get; }

        public Transition(uint delay, uint flags, uint endTransition, ushort start = 0, ushort mid = 0, ushort end = 0)
        {
            Delay = delay;
            Flags = flags;
            EndTransition = endTransition;
            Start = start;
            Mid = mid;
            End = end;
        }

        public void Send(WorldSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicTransition
            {
                Delay = Delay,
                Flags = Flags,
                EndTran = EndTransition,
                TranDurationStart = Start,
                TranDurationMid = Mid,
                TranDurationEnd = End
            });
        }
    }
}
