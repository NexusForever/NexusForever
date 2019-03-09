using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientPetCustomization, MessageDirection.Client)]
    public class ClientPetCustomization : IReadable
    {
        public PetType PetType { get; private set; }
        public uint Spell4Id { get; private set; }
        public ushort FlairSlotIndex { get; private set; }
        public ushort PetFlairId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PetType         = reader.ReadEnum<PetType>(2u);
            Spell4Id        = reader.ReadUInt();
            FlairSlotIndex  = reader.ReadUShort();
            PetFlairId      = reader.ReadUShort(14u);
        }
    }
}
