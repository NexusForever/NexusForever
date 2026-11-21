using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Guild
{
    public class RecruitmentGuildInfo : IWritable
    {
        public ulong GuildId { get; set; }
        public string GuildName { get; set; }
        public string GuildMasterName { get; set; }
        public GuildStats Stats { get; set; }
        public List<string> Recruiters { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GuildId);
            writer.WriteStringWide(GuildName);
            writer.WriteStringWide(GuildMasterName);
            Stats.Write(writer);

            writer.Write(Recruiters.Count);
            Recruiters.ForEach(s => writer.WriteStringWide(s));
        }
    }
}
