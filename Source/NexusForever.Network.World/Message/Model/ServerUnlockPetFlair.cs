using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerUnlockPetFlair)]
    public class ServerUnlockPetFlair : IWritable
    {
        public ushort PetFlairId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PetFlairId, 14u);
        }
    }
}

