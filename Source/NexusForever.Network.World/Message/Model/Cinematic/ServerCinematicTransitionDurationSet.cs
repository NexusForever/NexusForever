using NexusForever.Game.Static.Cinematic;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicScaleTransition)]
    public class ServerCinematicTransitionDurationSet : IWritable
    {
        public ScaleTransitionType Type { get; set; }
        public ushort DurationStart { get; set; }
        public ushort DurationMid { get; set; }
        public ushort DurationEnd { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Type, 32u);
            writer.Write(DurationStart);
            writer.Write(DurationMid);
            writer.Write(DurationEnd);
        }
    }
}
