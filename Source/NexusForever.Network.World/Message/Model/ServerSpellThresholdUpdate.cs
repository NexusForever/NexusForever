using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellThresholdUpdate)]
    public class ServerSpellThresholdUpdate : IWritable
    {
        public uint Spell4Id { get; set; }
        public byte Unknown0 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Spell4Id, 18u);
            writer.Write(Unknown0);
        }
    }
}
