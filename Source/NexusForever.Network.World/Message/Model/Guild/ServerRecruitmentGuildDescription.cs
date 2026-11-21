using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerRecruitmentGuildDescription)]
    public class ServerRecruitmentGuildDescription : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public string Description { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.WriteStringWide(Description);
        }
    }
}
