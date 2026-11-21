using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ClientHousingCommunityRename)]
    public class ClientHousingCommunityRename : IReadable
    {
        public Identity TargetGuild { get; } = new();
        public string Name { get; private set; }
        public bool UseServiceToken { get; private set; } // Uses ServiceToken

        public void Read(GamePacketReader reader)
        {
            TargetGuild.Read(reader);
            Name = reader.ReadWideString();
            UseServiceToken = reader.ReadBit();
        }
    }
}
