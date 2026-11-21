using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    [Message(GameMessageOpcode.ClientPathScientistDismissScanbotPathAction)]
    public class ClientPathScientistDismissScanbotPathAction : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // zero byte message
        }
    }
}
