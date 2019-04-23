using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientPetCustomisation)]
    public class ClientPetCustomisation : IReadable
    {
        public PetType PetType { get; private set; }
        public uint PetObjectId { get; private set; }
        public ushort FlairSlotIndex { get; private set; }
        public ushort FlairId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PetType        = reader.ReadEnum<PetType>(2u);
            PetObjectId    = reader.ReadUInt();
            FlairSlotIndex = reader.ReadUShort();
            FlairId        = reader.ReadUShort(14u);
        }
    }
}
