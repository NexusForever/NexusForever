using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
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
