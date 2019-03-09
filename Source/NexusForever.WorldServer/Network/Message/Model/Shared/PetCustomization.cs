using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class PetCustomization : IWritable
    {
        public const byte MaxSlots = 4;

        public PetType PetType { get; set; }
        public uint Spell4Id { get; set; }
        public string PetName { get; set; }
        // there are 4 slots - not sure if uint or some short/short struct
        public uint[] PetFlairIds { get; set; } = new uint[MaxSlots];

        public PetCustomization()
        {
            for (var i = 0; i < MaxSlots; i++)
                PetFlairIds[i] = 0;
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PetType, 2u);
            writer.Write(Spell4Id);
            writer.WriteStringWide(PetName);

            foreach (var slot in PetFlairIds)
                writer.Write(slot);
        }
    }
}