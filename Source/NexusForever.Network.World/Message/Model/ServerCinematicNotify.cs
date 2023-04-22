using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicNotify)]
    public class ServerCinematicNotify : IWritable
    {
        public ushort Flags { get; set; }
        public ushort Cancel { get; set; }
        public uint Duration { get; set; }
        public ushort CinematicId { get; set; } // 14

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Flags);
            writer.Write(Cancel);
            writer.Write(Duration);
            writer.Write(CinematicId, 14u);
        }
    }
}
