using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
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
