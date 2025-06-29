using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    // Sets the spell's activated state.  Spells are considered activated when they are able to be added to the player's limited action set.
    [Message(GameMessageOpcode.ClientAbilityBookActivateSpell)]
    public class ClientAbilityBookActivateSpell : IReadable
    {
        public uint Spell4Id { get; private set; }
        public bool Active { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Spell4Id = reader.ReadUInt(18u);
            Active = reader.ReadBit();
        }
    }
}
