using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity
{
    public class StatValue : IWritable
    {
        public enum StatType
        {
            Int,
            Float,
            Unknown
        }

        public Stat Stat { get; }
        public StatType Type { get; }
        public float Value { get; set; }

        public StatValue(Stat stat, uint value)
        {
            Stat  = stat;
            Type  = StatType.Int;
            Value = value;
        }

        public StatValue(Stat stat, float value)
        {
            Stat  = stat;
            Type  = StatType.Float;
            Value = value;
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Stat, 5);
            writer.Write(Type, 2);

            switch (Type)
            {
                case StatType.Int:
                    writer.Write((uint)Value);
                    break;
                case StatType.Float:
                    writer.Write(Value);
                    break;
                case StatType.Unknown:
                    writer.Write(0u);
                    writer.Write(0u);
                    break;
            }
        }
    }
}
