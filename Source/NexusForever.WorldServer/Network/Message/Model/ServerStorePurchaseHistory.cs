using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Account.Static;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerStorePurchaseHistory)]
    public class ServerStorePurchaseHistory : IWritable
    {
        public class Purchase : IWritable
        {
            public ulong PurchaseId { get; set; }
            public ulong TransactionDateTime { get; set; }
            public AccountCurrencyType CurrencyId { get; set; } // 5
            public byte Unknown0 { get; set; } // 3
            public float Cost { get; set; }
            public string Name { get; set; }
            public bool Unknown1 { get; set; }
            public ulong Unknown2 { get; set; }
            public uint Unknown3 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(PurchaseId);
                writer.Write(TransactionDateTime);
                writer.Write(CurrencyId, 5u);
                writer.Write(Unknown0, 3u);
                writer.Write(Cost);
                writer.WriteStringWide(Name);
                writer.Write(Unknown1);
                writer.Write(Unknown2);
                writer.Write(Unknown3);
            }
        }

        public List<Purchase> Purchases { get; set; } = new List<Purchase>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Purchases.Count);
            Purchases.ForEach(i => i.Write(writer));
        }
    }
}
