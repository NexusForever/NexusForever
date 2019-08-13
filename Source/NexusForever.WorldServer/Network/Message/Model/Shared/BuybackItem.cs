using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class BuybackItem : IWritable
    {
        public uint UniqueId { get; set; }
        public uint ItemId { get; set; }
        public uint Quantity { get; set; }
        public ulong Unk3 { get; set; }
        public uint Unk4 { get; set; }
        public ulong Unk5 { get; set; }
        public ulong Unk6 { get; set; }
        public uint Unk7 { get; set; }
        public byte[] Unk8 { get; set; } = new byte[20];
        public byte[] Unk9 { get; set; } = new byte[32];
        public CurrencyType[] CurrencyTypeId { get; set; } = new CurrencyType[2];
        public ulong[] CurrencyAmount { get; set; } = new ulong[2];
        public uint UnkE { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UniqueId);
            writer.Write(ItemId, 18u);
            writer.Write(Quantity);
            writer.Write(Unk3);
            writer.Write(Unk4);
            writer.Write(Unk5);
            writer.Write(Unk6);
            writer.Write(Unk7, 18u);
            writer.WriteBytes(Unk8, 20u);
            writer.WriteBytes(Unk9, 32u);

            writer.Write(CurrencyAmount, 2u);
            foreach (CurrencyType type in CurrencyTypeId)
                writer.Write(type, 4u);

            writer.Write(UnkE);
        }
    }
}
