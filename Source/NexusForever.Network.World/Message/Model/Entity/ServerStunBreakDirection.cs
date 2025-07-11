using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Entity
{
    // Sends the direction the player must press to break out of a stun.
    [Message(GameMessageOpcode.ServerStunBreakDirection)]
    public class ServerStunBreakDirection : IWritable
    {
        public CCStateStunVictimGameplay StunEscapeDirection { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(StunEscapeDirection, 8u);
        }
    }
}
