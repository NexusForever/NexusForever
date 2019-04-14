using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientToggleWeapons)]
    public class ClientToggleWeapons : IReadable
    {
        public bool ToggleState { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ToggleState = reader.ReadBit();
        }
    }
}
