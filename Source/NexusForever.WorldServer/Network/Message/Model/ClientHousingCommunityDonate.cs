using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingCommunityDonate)]
    public class ClientHousingCommunityDonate : IReadable
    {
        public List<DecorInfo> Decor { get; private set; } = new();

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
