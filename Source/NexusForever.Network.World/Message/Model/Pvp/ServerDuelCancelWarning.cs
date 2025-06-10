using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pvp
{
    // Fires whenever a player re-enters the dueling area while the LeftArea warning is displayed.
    [Message(GameMessageOpcode.ServerDuelCancelWarning)]
    public class ServerDuelCancelWarning : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // Zero byte message
        }
    }
}
