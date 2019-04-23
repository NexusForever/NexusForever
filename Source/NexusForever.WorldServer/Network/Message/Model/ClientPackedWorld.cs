using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientPackedWorld)]
    public class ClientPackedWorld : IReadable
    {
        // TODO: research this more
        // client hardcoded value of either 11 or 19 depending on what function sends the packet
        public byte Unknown0 { get; private set; }
        public byte[] Data { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Unknown0 = reader.ReadByte(5u);
            reader.ResetBits();

            uint length = reader.ReadUInt();
            Data = reader.ReadBytes(length - 4);
        }
    }
}
