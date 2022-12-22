using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPetCustomisation)]
    public class ServerPetCustomisation : IWritable
    {
        public PetCustomisation PetCustomisation { get; set; }

        public void Write(GamePacketWriter writer)
        {
            PetCustomisation.Write(writer);
        }
    }
}
