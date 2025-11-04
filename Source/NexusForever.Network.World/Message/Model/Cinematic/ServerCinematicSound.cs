using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicSound)]
    public class ServerCinematicSound : IWritable
    {
        public uint Delay { get; set; }
        public uint LocalizedTextId { get; set; }
        public bool Unused { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(LocalizedTextId, 21u);
            writer.Write(Unused);
        }
    }
}
