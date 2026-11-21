using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    [Message(GameMessageOpcode.ClientCinematicWhiteOutFinished)]
    public class ClientCinematicWhiteOutFinished : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // zero byte message
        }
    }
}
