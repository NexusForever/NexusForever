using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Network.Session;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Cinematic
{
    public class CameraTransition : ICameraTransition
    {
        public uint Delay { get; set; }
        public uint Type { get; set; }
        public ushort DurationStart { get; set; }
        public ushort DurationMid { get; set; }
        public ushort DurationEnd { get; set; }

        public CameraTransition(uint delay, uint type, ushort start = 1500, ushort mid = 0, ushort end = 1500)
        {
            Delay         = delay;
            Type          = type;
            DurationStart = start;
            DurationMid   = mid;
            DurationEnd   = end;
        }

        public void Send(IGameSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicCameraTransition
            {
                Delay         = Delay,
                Type          = Type,
                DurationStart = DurationStart,
                DurationMid   = DurationMid,
                DurationEnd   = DurationEnd
            });
        }
    }
}
