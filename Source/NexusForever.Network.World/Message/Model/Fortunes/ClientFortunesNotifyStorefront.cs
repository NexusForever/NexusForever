using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Fortunes
{
    // Sent to the storefront server when the player has opened the Fortunes screen
    // Sent at the same time as ClientFortunesNotifyGame
    [Message(GameMessageOpcode.ClientFortunesNotifyStorefront)]
    public class ClientFortunesNotifyStorefront : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // zero byte message
        }
    }
}
