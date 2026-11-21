using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Utility
{
    [Message(GameMessageOpcode.ClientInspectPlayerRequest)]
    public class ClientInspectPlayerRequest : IReadable
    {
        public uint UnitId { get; set; }

        public void Read(GamePacketReader reader)
        {
            UnitId = reader.ReadUInt();
        }
    }
}
