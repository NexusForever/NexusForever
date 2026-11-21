using NexusForever.Game.Static.Costume;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Costume
{
    public class Costume : IWritable
    {
        public const byte MaxCostumeItems = 7;

        public uint Index { get; set; } // zero indexed, for CostumeType.Personal the limit is 12, for CostumeType.Mannequin the limit is 5
        public uint VisibilityMask { get; set; } // Bitmask representing which costume slots are visible, uses CostumeItemSlot as bit index
        public CostumeType Type { get; set; }
        public uint[] Item2Ids { get; set; } = new uint[7]; // in order of CostumeItemSlot enum
        public uint[] DyeData { get; set; } = new uint[7]; // in order of CostumeItemSlot enum

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Index);
            writer.Write(VisibilityMask);
            writer.Write(Type, 2u);

            for (int i = 0; i < 7; i++)
                writer.Write(Item2Ids[i]);
            for (int i = 0; i < 7; i++)
                writer.Write(DyeData[i]);
        }
    }
}
