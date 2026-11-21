using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ClientRecruitmentGuildGetDetailedGuildInfo)]
    public class ClientRecruitmentGuildGetDetailedGuildInfo : IReadable
    {
        public Identity GuildIdentity { get; private set; } = new();

        public void Read(GamePacketReader reader)
        {
            GuildIdentity.Read(reader);
        }
    }
}
