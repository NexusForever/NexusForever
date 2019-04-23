using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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

