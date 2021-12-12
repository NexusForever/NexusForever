using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class EffectInfo : IWritable
    {
        public uint Spell4EffectId { get; set; }
        public uint EffectUniqueId { get; set; }
        public uint DelayTime { get; set; }
        public int TimeRemaining { get; set; }
        public byte InfoType { get; set; }

        public DamageDescription DamageDescriptionData { get; set; } = new DamageDescription();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Spell4EffectId, 19u);
            writer.Write(EffectUniqueId);
            writer.Write(DelayTime);
            writer.Write(TimeRemaining);
            writer.Write(InfoType, 2u);

            if (InfoType == 1)
                DamageDescriptionData.Write(writer);
            else
                writer.Write(0u, 1u);
        }
    }
}