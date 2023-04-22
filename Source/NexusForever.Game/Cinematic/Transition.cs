﻿using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Network;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Cinematic
{
    public class Transition : ITransition
    {
        public uint Delay { get; }
        public uint Flags { get; }
        public uint EndTransition { get; }
        public ushort Start { get; }
        public ushort Mid { get; }
        public ushort End { get; }

        public Transition(uint delay, uint flags, uint endTransition, ushort start = 0, ushort mid = 0, ushort end = 0)
        {
            Delay         = delay;
            Flags         = flags;
            EndTransition = endTransition;
            Start         = start;
            Mid           = mid;
            End           = end;
        }

        public void Send(IGameSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicTransition
            {
                Delay             = Delay,
                Flags             = Flags,
                EndTran           = EndTransition,
                TranDurationStart = Start,
                TranDurationMid   = Mid,
                TranDurationEnd   = End
            });
        }
    }
}
