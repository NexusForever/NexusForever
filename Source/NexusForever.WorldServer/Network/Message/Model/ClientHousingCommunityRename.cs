using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingCommunityRename)]
    public class ClientHousingCommunityRename : IReadable
    {
        public TargetGuild TargetGuild { get; } = new();
        public string Name { get; private set; }
        public bool AlternativeCurrency { get; private set; }

        public void Read(GamePacketReader reader)
        {
            TargetGuild.Read(reader);
            Name                = reader.ReadWideString();
            AlternativeCurrency = reader.ReadBit();
        }
    }
}
