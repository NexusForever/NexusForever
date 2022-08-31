using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicTransitionDurationSet)]
    public class ServerCinematicTransitionDurationSet : IWritable
    {
        public uint Type { get; set; }
        public ushort DurationStart { get; set; }
        public ushort DurationMid { get; set; }
        public ushort DurationEnd { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Type);
            writer.Write(DurationStart);
            writer.Write(DurationMid);
            writer.Write(DurationEnd);
        }
    }
}
