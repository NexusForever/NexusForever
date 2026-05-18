using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pet
{
    [Message(GameMessageOpcode.ServerPetSpawned)]
    public class ServerPetSpawned : IWritable
    {
        public uint PetUnitId { get; set; } // 0 means all engineer pets
        public uint SummoningSpell4Id { get; set; }
        public uint ValidStances { get; set; }
        public uint Stance { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PetUnitId);
            writer.Write(SummoningSpell4Id, 18u);
            writer.Write(ValidStances, 5u);
            writer.Write(Stance, 5u);
        }
    }
}
