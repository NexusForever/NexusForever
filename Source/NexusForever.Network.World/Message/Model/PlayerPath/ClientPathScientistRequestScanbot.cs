using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    // Sent when PlayerPathLib::PathAction or GameCommand is called, the active path is Scientist and there is no scanbot present
    [Message(GameMessageOpcode.ClientPathScientistRequestScanbot)]
    public class ClientPathScientistRequestScanbot : IReadable
    {
        public uint ScanbotProfile { get; private set; }
        public bool IsGameCommand { get; private set; } // 0 = triggered by Lua function call, 1 = triggered by GameCommand

        public void Read(GamePacketReader reader)
        {
            ScanbotProfile = reader.ReadUInt();
            IsGameCommand = reader.ReadBit();
        }
    }
}
