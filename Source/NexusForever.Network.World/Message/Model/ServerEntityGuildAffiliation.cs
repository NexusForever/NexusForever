using NexusForever.Game.Static.Guild;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityGuildAffiliation)]
    public class ServerEntityGuildAffiliation : IWritable
    {
        public uint UnitId { get; set; }
        public string GuildName { get; set; }
        public GuildType GuildType { get; set; } // 4

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.WriteStringWide(GuildName);
            writer.Write(GuildType, 4u);
        }
    }
}
