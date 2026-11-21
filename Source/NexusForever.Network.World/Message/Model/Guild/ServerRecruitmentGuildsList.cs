using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerRecruitmentGuildsList)]
    public class ServerRecruitmentGuildsList : IWritable
    {
        public List<RecruitmentGuildInfo> Guilds { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guilds.Count);
            Guilds.ForEach(guild => guild.Write(writer));
        }
    }
}
