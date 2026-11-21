using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerRecruitmentGuildsUpdate)]
    public class ServerRecruitmentGuildsUpdate : IWritable
    {
        public List<RecruitmentGuildInfo> Guilds { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guilds.Count);
            for (int i = 0; i < Guilds.Count; i++)
            {
                var guild = Guilds[i];
                writer.Write(guild.GuildId);
                writer.WriteStringWide(guild.GuildName);
                writer.WriteStringWide(guild.GuildMasterName);
                guild.Stats.Write(writer);
            }
        }
    }
}
