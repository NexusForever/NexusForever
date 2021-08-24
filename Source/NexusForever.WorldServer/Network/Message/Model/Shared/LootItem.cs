using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Loot.Static;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class LootItem : IWritable
    {
        public int UniqueId { get; set; }
        public LootItemType Type { get; set; }
        public uint StaticId { get; set; }
        public uint Amount { get; set; }
        public bool CanLoot { get; set; }
        public bool NeedsRoll { get; set; }
        public bool Explosion { get; set; }
        public bool Granted { get; set; }
        public uint RollTime { get; set; }
        public ulong RandomCircuitData { get; set; }
        public uint RandomGlyphData { get; set; }
        public uint Unknown2 { get; set; }
        public List<TargetPlayerIdentity> MasterList { get; set; } = new List<TargetPlayerIdentity>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UniqueId);
            writer.Write(Type, 32u);
            writer.Write(StaticId);
            writer.Write(Amount);
            writer.Write(CanLoot);
            writer.Write(NeedsRoll);
            writer.Write(Explosion);
            writer.Write(Granted);
            writer.Write(RollTime);
            writer.Write(RandomCircuitData);
            writer.Write(RandomGlyphData);
            writer.Write(Unknown2);
            writer.Write(MasterList.Count);
            MasterList.ForEach(m => m.Write(writer));
        }
    }
}
