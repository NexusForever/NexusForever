using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    // Fires when the player is forced to resurrect by a scripted event in game.
    [Message(GameMessageOpcode.ServerScriptResurrect)]
    public class ServerScriptResurrect : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // Zero byte message
        }
    }
}
