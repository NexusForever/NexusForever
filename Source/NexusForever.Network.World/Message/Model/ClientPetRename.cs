using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPetRename)]
    public class ClientPetRename : IReadable
    {
        public PetType PetType { get; private set; }
        public uint PetObjectId { get; private set; }
        public String Name { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PetType = reader.ReadEnum<PetType>(2u);
            PetObjectId = reader.ReadUInt();
            Name = reader.ReadWideString();
        }
    }
}
