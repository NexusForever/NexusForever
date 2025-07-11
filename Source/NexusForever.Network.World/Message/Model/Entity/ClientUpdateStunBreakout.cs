using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Entity
{
    // May send this multiple times while the player is stunned.
    [Message(GameMessageOpcode.ClientUpdateStunBreakout)]
    public class ClientUpdateStunBreakout : IReadable
    {
        public CCStateStunVictimGameplay StunBreakoutDirectionInputRising { get; private set; }
        public CCStateStunVictimGameplay StunBreakoutDirectionInputDown { get; private set; }

        public void Read(GamePacketReader reader)
        {
            StunBreakoutDirectionInputRising = reader.ReadEnum<CCStateStunVictimGameplay>(8u); // The direction inputs considered 'key up'
            StunBreakoutDirectionInputDown = reader.ReadEnum<CCStateStunVictimGameplay>(8u); // The directions inputs considered 'key down'
        }
    }
}
