using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    // Sent by PlayerPathLib::ScientistToggleScanbot, PlayerPathLib::PathAction, or PlayerPathLib::PathAction2 lua functions
    [Message(GameMessageOpcode.ClientPathScientistDismissScanbot)]
    public class ClientPathScientistDismissScanbot : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // zero byte message
        }
    }
}
