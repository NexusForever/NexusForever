using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerRecruitmentGuildDemands)]
    public class ServerRecruitmentGuildDemands : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public RecruitmentDemands Demands { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            Demands.Write(writer);
        }
    }
}

