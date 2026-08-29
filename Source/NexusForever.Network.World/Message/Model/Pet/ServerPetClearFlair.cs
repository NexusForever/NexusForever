using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pet
{
    [Message(GameMessageOpcode.ServerPetClearFlair)]
    public class ServerPetClearFlair : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // Zero byte message
        }
    }
}
