using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerRecruitmentGuildsRecruiters)]
    public class ServerRecruitmentGuildsRecruiters : IWritable
    {
        public class GuildRecruiters : IWritable
        {
            public ulong GuildId { get; set; }
            public bool IsRecruiting { get; set; }
            public List<string> RecruiterNames { get; set; } = [];
            public List<bool> IsOnline { get; set; } = []; // isOnline state of the recruiter

            public void Write(GamePacketWriter writer)
            {
                writer.Write(GuildId);
                writer.Write(IsRecruiting);

                writer.Write(RecruiterNames.Count);
                RecruiterNames.ForEach(s => writer.WriteStringWide(s));
                IsOnline.ForEach(b => writer.Write(b));
            }
        }

        public List<GuildRecruiters> RecruitingGuilds { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RecruitingGuilds.Count);
            RecruitingGuilds.ForEach(guild => guild.Write(writer));
        }
    }
}
