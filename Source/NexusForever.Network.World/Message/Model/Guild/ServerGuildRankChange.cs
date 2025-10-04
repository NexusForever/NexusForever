using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildRankChange)]
    public class ServerGuildRankChange : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public List<GuildRank> Ranks { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            if (Ranks.Count < 10)
                for (int i = Ranks.Count; i < 10; i++)
                    Ranks.Add(new GuildRank());
            Ranks.ForEach(c => c.Write(writer));
        }
    }
}
