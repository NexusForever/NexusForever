using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Storefront.Static;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerStoreOffers)]
    public class ServerStoreOffers : IWritable
    {
        public class OfferGroup : IWritable
        {
            public class Offer : IWritable
            {
                public class OfferCurrencyData : IWritable
                {
                    public byte CurrencyId { get; set; } // 5
                    public float Price { get; set; }
                    public DiscountType DiscountType { get; set; } // 2
                    public float DiscountValue { get; set; }
                    public long DiscountTimeRemaining { get; set; }
                    public long TimeSinceExpiry { get; set; }

                    public void Write(GamePacketWriter writer)
                    {
                        writer.Write(CurrencyId, 5u);
                        writer.Write(Price);
                        writer.Write(DiscountType, 2u);
                        writer.Write(DiscountValue);
                        writer.Write(DiscountTimeRemaining);
                        writer.Write(TimeSinceExpiry);
                    }
                }

                public class OfferItemData : IWritable
                {
                    public uint Type { get; set; } // Should be 0
                    public ushort AccountItemId { get; set; } // 15
                    public uint Amount { get; set; }

                    public uint Type1Unknown0 { get; set; } = 0;
                    public uint Type1Unknown1 { get; set; } = 0;

                    public uint Type2Unknown0 { get; set; } = 0;

                    public void Write(GamePacketWriter writer)
                    {
                        writer.Write(Type);
                        if (Type == 0)
                        {
                            writer.Write(AccountItemId, 15u);
                            writer.Write(Amount);
                        }
                        else if (Type == 1)
                        {
                            writer.Write(Type1Unknown0);
                            writer.Write(Type1Unknown1);
                        }
                        else if (Type == 2)
                        {
                            writer.Write(Type2Unknown0);
                        }
                    }
                }

                public uint Id { get; set; }
                public string OfferName { get; set; }
                public string OfferDescription { get; set; }
                public float PriceProtobucks { get; set; }
                public float PriceOmnibits { get; set; }
                public DisplayFlag DisplayFlags { get; set; }
                public long Unknown6 { get; set; } 
                public byte Unknown7 { get; set; } 
                public List<OfferCurrencyData> CurrencyData { get; set; } = new List<OfferCurrencyData>();
                public List<OfferItemData> ItemData { get; set; } = new List<OfferItemData>();

                public void Write(GamePacketWriter writer)
                {
                    writer.Write(Id);
                    writer.WriteStringWide(OfferName);
                    writer.WriteStringWide(OfferDescription);

                    writer.Write(PriceProtobucks);
                    writer.Write(PriceOmnibits);

                    writer.Write(DisplayFlags, 32u);
                    writer.Write(Unknown6);
                    writer.Write(Unknown7);

                    writer.Write(CurrencyData.Count);
                    CurrencyData.ForEach(e => e.Write(writer));

                    writer.Write(ItemData.Count);
                    ItemData.ForEach(e => e.Write(writer));
                }
            }

            public uint Id { get; set; }
            public DisplayFlag DisplayFlags { get; set; }
            public string OfferGroupName { get; set; }
            public string OfferGroupDescription { get; set; }
            public ushort Unknown2 { get; set; }
            public List<Offer> Offers { get; set; } = new List<Offer>();
            public uint ArraySize { get; set; }
            public uint[] CategoryArray { get; set; }
            public uint[] CategoryIndexArray { get; set; }


            public void Write(GamePacketWriter writer)
            {
                writer.Write(Id);
                writer.Write(DisplayFlags, 32u);
                writer.WriteStringWide(OfferGroupName);
                writer.WriteStringWide(OfferGroupDescription);
                writer.Write(Unknown2, 14u);

                writer.Write(Offers.Count);
                Offers.ForEach(e => e.Write(writer));

                writer.Write(ArraySize);
                foreach (uint category in CategoryArray)
                    writer.Write(category);
                foreach (uint index in CategoryIndexArray)
                    writer.Write(index);
            }
        }

        public List<OfferGroup> OfferGroups { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(OfferGroups.Count);
            OfferGroups.ForEach(e => e.Write(writer));
        }
    }
}