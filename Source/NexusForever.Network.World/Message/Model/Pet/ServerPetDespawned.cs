using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPetDespawned)]
    public class ServerPetDespawned : IWritable
    {
        public uint PetUnitId { get; set; } // TBC if this follows other messages, 0 means all engineer pets

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PetUnitId);
        }
    }
}
