using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    // Fires when the player's automatic release timer runs out while they are dead.
    [Message(GameMessageOpcode.ServerForceResurrect)]
    public class ServerForceResurrect : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // Zero byte message
        }
    }
}
