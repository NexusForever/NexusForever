using NexusForever.WorldServer.Game.Cinematic.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Cinematic
{
    public class CameraTransition : IKeyframeAction
    {
        public uint Delay { get; set; }
        public uint Type { get; set; }
        public ushort DurationStart { get; set; }
        public ushort DurationMid { get; set; }
        public ushort DurationEnd { get; set; }

        public CameraTransition(uint delay, uint type, ushort start = 1500, ushort mid = 0, ushort end = 1500)
        {
            Delay = delay;
            Type = type;
            DurationStart = start;
            DurationMid = mid;
            DurationEnd = end;
        }

        public void Send(WorldSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicCameraTransition
            {
                Delay = Delay,
                Type = Type,
                DurationStart = DurationStart,
                DurationMid = DurationMid,
                DurationEnd = DurationEnd
            });
        }
    }
}
