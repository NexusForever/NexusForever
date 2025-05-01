using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerProfessionModifiers)]
    public class ServerProfessionModifiers : IWritable
    {
        public class CraftingModifier : IWritable
        {
            CraftingModifierType Type { get; set; }
            TradeskillType TradeskillId { get; set; }
            uint Item2TypeId { get; set; }
            uint Item2MaterialId { get; set; }
            float Coefficient { get; set; } // Might be used additive or multiplier, depends on Type
            uint FixedValue { get; set; }

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

        public List<CraftingModifier> Modifiers { get; set; } = new List<CraftingModifier>();

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
