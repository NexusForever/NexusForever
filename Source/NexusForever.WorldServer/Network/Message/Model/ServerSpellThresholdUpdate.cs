using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellThresholdUpdate)]
    public class ServerSpellThresholdUpdate : IWritable
    {
        public uint Spell4Id { get; set; }
        public byte Value { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Spell4Id, 18u);
            writer.Write(Value);
        }
    }
}
