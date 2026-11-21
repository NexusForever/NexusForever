using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ClientRecruitmentGuildSubscribe)]
    public class ClientRecruitmentGuildSubscribe : IReadable
    {
        public bool Subscribe { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Subscribe = reader.ReadBit();
        }
    }
}
