using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Spell.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerGuildTaxUpdate)]
    public class ServerGuildTaxUpdate : IWritable
    {
        public ushort RealmId { get; set; }
        public ulong GuildId { get; set; }
        public uint Value { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RealmId, 14u);
            writer.Write(GuildId);
            writer.Write(Value);
        }
    }
}
