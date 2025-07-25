using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pet
{
    [Message(GameMessageOpcode.ServerClearPetFlair)]
    public class ServerClearPetFlair : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // Zero byte message
        }
    }
}
