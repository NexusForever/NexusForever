using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientToggleWeapons, MessageDirection.Client)]
    public class ClientToggleWeapons : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            ToggleState = reader.ReadByte(1);
        }

        public byte ToggleState { get; private set; }
    }
}