using NexusForever.Game.Static.Pet;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pet
{
    [Message(GameMessageOpcode.ServerPetStanceChanged)]
    public class ServerPetStanceChanged : IWritable
    {
        public uint PetUnitId { get; set; } // 0 means all engineer pets
        public PetStance Stance { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PetUnitId);
            writer.Write(Stance, 5u);
        }
    }
}
