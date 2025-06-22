using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pvp
{
    // Sets the players PVP cooldown to zero
    [Message(GameMessageOpcode.ServerPvpCooldownClear)]
    public class ServerPvpCooldownClear : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // Zero byte message
        }
    }
}
