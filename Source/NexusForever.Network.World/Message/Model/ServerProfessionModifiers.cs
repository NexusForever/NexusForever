using NexusForever.Network.Message;
using NexusForever.Game.Static.Crafting;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerProfessionModifiers)]
    public class ServerProfessionModifiers : IWritable
    {
        public class CraftingModifier : IWritable
        {
            public CraftingModifierType Type { get; set; }
            public TradeskillType TradeskillId { get; set; }
            public uint Item2TypeId { get; set; }
            public uint Item2MaterialId { get; set; }
            public float Coefficient { get; set; } // Might be used additive or multiplier, depends on Type
            public uint FixedValue { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Type, 32u);
                writer.Write(TradeskillId);
                writer.Write(Item2TypeId);
                writer.Write(Item2MaterialId);
                writer.Write(Coefficient);
                writer.Write(FixedValue);
            }
        }

        public List<CraftingModifier> Modifiers { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Modifiers.Count);
            foreach (var modifier in Modifiers)
            {
                modifier.Write(writer);
            }
        }
    }
}
