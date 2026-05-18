using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class PetCustomisation : IWritable
    {
        public const byte MaxCustomisationFlairs = 4;

        public PetType PetType { get; set; }
        public uint PetObjectId { get; set; }
        public string PetName { get; set; }
        // there are 4 slots - not sure if uint or some short/short struct
        public uint[] SlotFlairIds { get; set; } = new uint[MaxCustomisationFlairs];

        public PetCustomisation()
        {
            for (int i = 0; i < SlotFlairIds.Length; i++)
                SlotFlairIds[i] = 0;
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PetType, 2u);
            writer.Write(PetObjectId);
            writer.WriteStringWide(PetName);

            foreach(uint slot in SlotFlairIds)
                writer.Write(slot);
        }
    }
}
