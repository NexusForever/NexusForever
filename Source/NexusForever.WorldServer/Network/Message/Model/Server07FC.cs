using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server07FC, MessageDirection.Server)]
    public class Server07FC : IWritable
    {
        public uint Unknown0 { get; set; }
        public uint Spell4Id { get; set; }
        public ushort Unknown9 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
            writer.Write(Spell4Id, 18u);
            writer.Write(Unknown9, 9u);
        }
    }
}
