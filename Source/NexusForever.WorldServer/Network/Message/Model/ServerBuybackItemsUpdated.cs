using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerBuybackItemsUpdated, MessageDirection.Server)]
    public class ServerBuybackItemsUpdated : IWritable
    {
        public BuybackItem BuybackItem { get; set;}
        public ulong Unk3 { get; set; }
        public uint Unk4 { get; set; }
        public ulong Unk5 { get; set; }
        public ulong Unk6 { get; set; }
        public uint Unk7 { get; set; }
        public byte[] Unk8 { get; set; } = new byte[20];
        public byte[] Unk9 { get; set; } = new byte[32];
        public uint UnkE { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(BuybackItem.BuybackItemId);
            writer.Write(BuybackItem.Item.Entry.Id, 18);
            writer.Write(BuybackItem.Quantity);
            writer.Write(Unk3);
            writer.Write(Unk4);
            writer.Write(Unk5);
            writer.Write(Unk6);
            writer.Write(Unk7, 18);
            writer.WriteBytes(Unk8, 20);
            writer.WriteBytes(Unk9, 32);
            foreach (KeyValuePair<CurrencyTypeEntry, ulong> entry in BuybackItem.CurrencyAdditions)
                writer.Write(entry.Value);
            for (int i = 0; i < 2 - BuybackItem.CurrencyAdditions.Count; i++)
                writer.Write(0ul);
            foreach (KeyValuePair<CurrencyTypeEntry, ulong> entry in BuybackItem.CurrencyAdditions)
                writer.Write(entry.Key.Id);
            for (int i = 0; i < 2 - BuybackItem.CurrencyAdditions.Count; i++)
                writer.Write(0ul);
            writer.Write(UnkE);
        }
    }
}
