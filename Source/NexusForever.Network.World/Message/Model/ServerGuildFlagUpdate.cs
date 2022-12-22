using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGuildFlagUpdate)]
    public class ServerGuildFlagUpdate : IWritable
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
