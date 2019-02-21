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

        public Stat Stat { get; private set; }
        public StatType Type { get; }
        public float Value { get; set; }
        public StatSaveMask SaveMask { get; set; }

        public StatValue(Stat stat, uint value)
        {
            Stat  = stat;
            Type = GetStatType(stat);
            Value = value;
        }

        public StatValue(Stat stat, float value)
        {
            Stat  = stat;
            Type = GetStatType(stat);
            Value = value;
        }

        public static StatType GetStatType(Stat stat)
        {
            switch(stat)
            {
                case Stat.Health:
                case Stat.Level:
                case Stat.MentorLevel:
                case Stat.StandState:
                case Stat.Sheathed:
                case (Stat)17:
                case Stat.Shield:
                case (Stat)21:
                case (Stat)22:
                case (Stat)26:
                    return StatType.Int;

                case Stat.Focus:
                case Stat.Resource0:
                case Stat.Resource1:
                case Stat.Resource2:
                case Stat.Resource3:
                case Stat.Resource4:
                case Stat.Resource5:
                case Stat.Resource6:
                case Stat.Dash:
                case (Stat)19:
                case (Stat)23:
                case (Stat)24:
                case (Stat)25:
                    return StatType.Float;

                default:
                    return StatType.Unknown;
            }
        }

        public static bool SendUpdate(Stat stat)
        {
            switch(stat)
            {
                case Stat.Focus:
                case Stat.Resource0:
                case Stat.Resource1:
                case Stat.Resource2:
                case Stat.Resource3:
                case Stat.Resource4:
                case Stat.Resource5:
                case Stat.Resource6:
                case Stat.Dash:
                case Stat.Level:
                case Stat.MentorLevel:
                case Stat.Sheathed:
                case (Stat)17:
                case (Stat)19:
                case Stat.Shield:
                case (Stat)21:
                case (Stat)22:
                case (Stat)23:
                case (Stat)24:
                case (Stat)25:
                case (Stat)26:
                    return true;

                case Stat.Health:
                default:
                    return false;
            }
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

        public void WriteUpdate(GamePacketWriter writer)
        {
            writer.Write(Stat, 5);

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
