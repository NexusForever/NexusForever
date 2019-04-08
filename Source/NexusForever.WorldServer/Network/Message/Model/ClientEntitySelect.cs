using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientEntitySelect)]
    public class ClientEntitySelect : IReadable
    {
        public uint Guid { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Guid = reader.ReadUInt();
        }
    }
}
