using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pet
{
    [Message(GameMessageOpcode.ClientPetCustomisation)]
    public class ClientPetCustomisation : IReadable
    {
        public PetType PetType { get; private set; }
        public uint PetObjectId { get; private set; } // PathScientistScanBotProfileId in the case of scanbots
        public ushort FlairSlotIndex { get; private set; }
        public ushort FlairId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PetType = reader.ReadEnum<PetType>(2u);
            PetObjectId = reader.ReadUInt();
            FlairSlotIndex = reader.ReadUShort();
            FlairId = reader.ReadUShort(14u);
        }
    }
}
