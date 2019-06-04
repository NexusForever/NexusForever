using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellThresholdStart)]
    public class ServerSpellThresholdStart : IWritable
    {
        public uint Spell4Id { get; set; }
        public uint RootSpell4Id { get; set; }
        public uint ParentSpell4Id { get; set; } = 0;
        public uint CastingId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Spell4Id, 18u);
            writer.Write(RootSpell4Id, 18u);
            writer.Write(ParentSpell4Id, 18u);
            writer.Write(CastingId);
        }
    }
}
