using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientInspectPlayerRequest)]
    public class ClientInspectPlayerRequest : IReadable
    {
        public uint Guid { get; set; }

        public void Read(GamePacketReader reader)
        {
            Guid = reader.ReadUInt();
        }
    }
}
