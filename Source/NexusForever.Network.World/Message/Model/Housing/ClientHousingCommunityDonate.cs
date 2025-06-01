using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ClientHousingCommunityDonate)]
    public class ClientHousingCommunityDonate : IReadable
    {
        public List<DecorInfo> Decor { get; private set; } = [];

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUInt();
            for (uint i = 0u; i < count; i++)
            {
                var decor = new DecorInfo();
                decor.Read(reader);
                Decor.Add(decor);
            }
        }
    }
}
