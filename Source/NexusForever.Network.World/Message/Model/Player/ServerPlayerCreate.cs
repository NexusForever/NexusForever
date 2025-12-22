using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using FactionId = NexusForever.Game.Static.Reputation.Faction;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ServerPlayerCreate)]
    public class ServerPlayerCreate : IWritable
    {
        public class Faction : IWritable
        {
            public class FactionReputation : IWritable
            {
                public FactionId FactionId { get; set; }
                public float Value { get; set; }

                public void Write(GamePacketWriter writer)
                {
                    writer.Write(FactionId, 14u);
                    writer.Write(Value);
                }
            }

            public FactionId FactionId { get; set; }
            public List<FactionReputation> FactionReputations { get; set; } = new();

            public void Write(GamePacketWriter writer)
            {
                writer.Write(FactionId, 14u);

                writer.Write((ushort)FactionReputations.Count);
                FactionReputations.ForEach(f => f.Write(writer));
            }
        }

        public class Pet : IWritable
        {
            public uint PetId { get; set; }
            public uint SummoningSpell { get; set; }
            public byte ValidStances { get; set; }
            public byte Stance { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(PetId);
                writer.Write(SummoningSpell, 18u);
                writer.Write(ValidStances, 5u);
                writer.Write(Stance, 5u);
            }
        }

        public class CharacterEntitlement : IWritable
        {
            public EntitlementType Entitlement { get; set; }
            public uint Count { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Entitlement, 14u);
                writer.Write(Count);
            }
        }

        public List<InventoryItem> Inventory { get; } = [];
        public ulong[] Money { get; } = new ulong[16];
        public uint Xp { get; set; }
        public uint RestBonusXp { get; set; }
        public ItemProficiency ItemProficiencies { get; set; }
        public uint ElderPoints { get; set; }
        public uint DailyElderPoints { get; set; }
        public byte SpecIndex { get; set; }
        public ushort BonusPower { get; set; }
        public uint BonusAbilityTierPoints { get; set; }
        public Faction FactionData { get; set; }
        public List<Pet> Pets { get; } = [];
        public uint InputKeySet { get; set; }
        public ushort BindPointId { get; set; }
        public int ActiveCostumeIndex { get; set; }
        public uint AttributePoints { get; set; }
        public uint PvpFlagDuration { get; set; }
        public List<ushort> KnownDyes { get; } = [];
        public ushort[] TradeskillMaterials { get; set; } = new ushort[512];
        public float GearScore { get; set; }
        public bool IsPvpServer { get; set; }
        public uint MatchingEligibilityFlagMask { get; set; } // Mask checked against matchingMapPreqrequisite.matchingEligibilityFlagEnum field. Tbl has no entries though.
        public List<CharacterEntitlement> CharacterEntitlements { set; get; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Inventory.Count);
            Inventory.ForEach(i => i.Write(writer));

            for (uint i = 0u; i < Money.Length; i++)
                writer.Write(Money[i]);

            writer.Write(Xp);
            writer.Write(RestBonusXp);
            writer.Write(ItemProficiencies, 32u);
            writer.Write(ElderPoints);
            writer.Write(DailyElderPoints);
            writer.Write(SpecIndex, 3u);
            writer.Write(BonusPower);
            writer.Write(BonusAbilityTierPoints);

            FactionData.Write(writer);

            writer.Write(Pets.Count);
            Pets.ForEach(p => p.Write(writer));

            writer.Write(InputKeySet);
            writer.Write(BindPointId);
            writer.Write(ActiveCostumeIndex);
            writer.Write(AttributePoints);
            writer.Write(PvpFlagDuration);

            writer.Write((byte)KnownDyes.Count, 6u);
            KnownDyes.ForEach(a => writer.Write(a));

            for (uint i = 0u; i < TradeskillMaterials.Length; i++)
                writer.Write(TradeskillMaterials[i]);

            writer.Write(GearScore);
            writer.Write(IsPvpServer);
            writer.Write(MatchingEligibilityFlagMask);

            writer.Write(CharacterEntitlements.Count);
            CharacterEntitlements.ForEach(u => u.Write(writer));
        }
    }
}
