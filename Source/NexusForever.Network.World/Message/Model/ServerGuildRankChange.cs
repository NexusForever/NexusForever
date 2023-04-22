using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGuildRankChange)]
    public class ServerGuildRankChange : IWritable
    {
        public ushort RealmId { get; set; }
        public ulong GuildId { get; set; }
        public List<GuildRank> Ranks { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RealmId, 14u);
            writer.Write(GuildId);
            if (Ranks.Count < 10)
                for (int i = Ranks.Count; i < 10; i++)
                    Ranks.Add(new GuildRank());
            Ranks.ForEach(c => c.Write(writer));
        }
    }
}
