using NexusForever.Game.Static.Combat.CrowdControl;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Entity
{
    // May send this multiple times while the player is stunned.
    [Message(GameMessageOpcode.ClientCCStateStunUpdate)]
    public class ClientCCStateStunUpdate : IReadable
    {
        public CCStateStunVictimGameplay InputPressed { get; private set; }
        public CCStateStunVictimGameplay InputHeld { get; private set; }

        public void Read(GamePacketReader reader)
        {
            InputPressed = reader.ReadEnum<CCStateStunVictimGameplay>(8u); // The direction inputs considered 'key up'
            InputHeld    = reader.ReadEnum<CCStateStunVictimGameplay>(8u); // The directions inputs considered 'key down'
        }
    }
}
