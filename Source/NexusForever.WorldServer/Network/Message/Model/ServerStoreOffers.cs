using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Storefront.Static;

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

                    public uint Type1Unknown0 { get; set; }
                    public uint Type1Unknown1 { get; set; }

                    public uint Type2Unknown0 { get; set; }

                    public void Write(GamePacketWriter writer)
                    {
                        writer.Write(Type);
                        switch (Type)
                        {
                            case 0:
                                writer.Write(AccountItemId, 15u);
                                writer.Write(Amount);
                                break;
                            case 1:
                                writer.Write(Type1Unknown0);
                                writer.Write(Type1Unknown1);
                                break;
                            case 2:
                                writer.Write(Type2Unknown0);
                                break;
                        }
                    }
                }

                public uint Id { get; set; }
                public string Name { get; set; }
                public string Description { get; set; }
                public float PricePremium { get; set; }
                public float PriceAlternative { get; set; }
                public DisplayFlag DisplayFlags { get; set; }
                public long Unknown6 { get; set; } 
                public byte Unknown7 { get; set; } 
                public List<OfferCurrencyData> CurrencyData { get; set; } = new List<OfferCurrencyData>();
                public List<OfferItemData> ItemData { get; set; } = new List<OfferItemData>();

                public void Write(GamePacketWriter writer)
                {
                    writer.Write(Id);
                    writer.WriteStringWide(Name);
                    writer.WriteStringWide(Description);

                    writer.Write(PricePremium);
                    writer.Write(PriceAlternative);

                    writer.Write(DisplayFlags, 32u);
                    writer.Write(Unknown6);
                    writer.Write(Unknown7);

                    writer.Write(CurrencyData.Count);
                    CurrencyData.ForEach(e => e.Write(writer));

                    writer.Write(ItemData.Count);
                    ItemData.ForEach(e => e.Write(writer));
                }
            }

            public struct Category
            {
                public uint Id { get; set; }
                public uint Index { get; set; }
            }

            public uint Id { get; set; }
            public DisplayFlag DisplayFlags { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public ushort DisplayInfoOverride { get; set; }
            public List<Offer> Offers { get; set; } = new List<Offer>();
            public List<Category> Categories { get; set; } = new List<Category>();

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Id);
                writer.Write(DisplayFlags, 32u);
                writer.WriteStringWide(Name);
                writer.WriteStringWide(Description);
                writer.Write(DisplayInfoOverride, 14u);

                writer.Write(Offers.Count);
                Offers.ForEach(e => e.Write(writer));

                writer.Write(Categories.Count);
                foreach (Category category in Categories)
                    writer.Write(category.Id);
                foreach (Category category in Categories)
                    writer.Write(category.Index);
            }
        }

        public List<OfferGroup> OfferGroups { get; set; } = new List<OfferGroup>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(OfferGroups.Count);
            OfferGroups.ForEach(e => e.Write(writer));
        }
    }
}
