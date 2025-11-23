using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Fortunes
{
    // Sent to the game server when the player has opened the Fortunes screen
    // Sent at the same time as ClientFortunesNotifyStorefront
    [Message(GameMessageOpcode.ClientFortunesNotifyGame)]
    public class ClientFortunesNotifyGame : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // zero byte message
        }
    }
}
