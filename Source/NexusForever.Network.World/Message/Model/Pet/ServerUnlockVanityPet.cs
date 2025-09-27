using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pet
{
    // Triggers VanityPetUnlocked ui event but there is no Carbine UI handler that uses this
    // Triggers chatlog message "You added $1n to your Collectibles." with $1n filled in from the spell4Id
    [Message(GameMessageOpcode.ServerUnlockVanityPet)]
    public class ServerUnlockVanityPet : IWritable
    {
        public uint Spell4Id { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Spell4Id, 18u);
        }
    }
}

