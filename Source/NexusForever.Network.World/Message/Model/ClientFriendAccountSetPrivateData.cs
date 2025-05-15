using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientFriendAccountSetPrivateData)]
    public class ClientFriendAccountSetPrivateData : IReadable
    {
        public ulong AccountFriendId { get; private set; }
        public uint Unused { get; private set; } // Always 2
        public string PrivateDataText { get; private set; }

        public void Read(GamePacketReader reader)
        {
            AccountFriendId = reader.ReadULong();
            Unused = reader.ReadUInt();
            PrivateDataText = reader.ReadWideString();
        }
    }
}
