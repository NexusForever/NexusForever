using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using FactionId = NexusForever.WorldServer.Game.Entity.Static.Faction;

namespace NexusForever.WorldServer.Network.Message.Model
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
            public List<FactionReputation> FactionReputations { get; } = new List<FactionReputation>();

            public void Write(GamePacketWriter writer)
            {
                writer.Write(FactionId, 14u);

                writer.Write((ushort)FactionReputations.Count);
                FactionReputations.ForEach(f => f.Write(writer));
            }
        }

        public class Pet : IWritable
        {
            public uint Guid { get; set; }
            public uint SummoningSpell { get; set; }
            public byte ValidStances { get; set; }
            public byte Stance { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Guid);
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

        public List<InventoryItem> Inventory { get; } = new List<InventoryItem>();
        public ulong[] Money { get; } = new ulong[16];
        public uint Xp { get; set; }
        public uint RestBonusXp { get; set; }
        public ItemProficiency ItemProficiencies { get; set; }
        public uint ElderPoints { get; set; }
        public uint DailyElderPoints { get; set; }
        public byte SpecIndex { get; set; }
        public ushort BonusPower { get; set; }
        public uint UnknownA0 { get; set; }
        public Faction FactionData { get; set; }
        public List<Pet> Pets { get; } = new List<Pet>();
        public uint InputKeySet { get; set; }
        public ushort UnknownBC { get; set; }
        public int ActiveCostumeIndex { get; set; }
        public uint UnknownC4 { get; set; }
        public uint UnknownC8 { get; set; }
        public List<ushort> KnownDyes { get; } = new List<ushort>();
        public byte[] TradeskillMaterials { get; } = new byte[1024];
        public float GearScore { get; set; }
        public bool IsPvpServer { get; set; }
        public uint Unknown4DC { get; set; }
        public List<CharacterEntitlement> CharacterEntitlements { set; get; } = new List<CharacterEntitlement>();

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
            writer.Write(UnknownA0);

            FactionData.Write(writer);

            writer.Write(Pets.Count);
            Pets.ForEach(p => p.Write(writer));

            writer.Write(InputKeySet);
            writer.Write(UnknownBC);
            writer.Write(ActiveCostumeIndex);
            writer.Write(UnknownC4);
            writer.Write(UnknownC8);

            writer.Write((byte)KnownDyes.Count, 6u);
            KnownDyes.ForEach(a => writer.Write(a));

            for (uint i = 0u; i < TradeskillMaterials.Length; i++)
                writer.Write(TradeskillMaterials[i]);

            writer.Write(GearScore);
            writer.Write(IsPvpServer);
            writer.Write(Unknown4DC);

            writer.Write(CharacterEntitlements.Count);
            CharacterEntitlements.ForEach(u => u.Write(writer));
        }
    }
}
