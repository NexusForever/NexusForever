using NexusForever.Network.Message;
using NexusForever.Network;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerUnitImmunity)]
    public class ServerUnitImmunity : IWritable
    {
        // TODO: Research more
        public enum ImmunityType
        {
            Spell        = 3,
            Melee        = 4,
            Ranged       = 5,
            Invulnerable = 6,
            SpellFlag1   = 7,
            MeleeFlag1   = 8,
            RangedFlag1  = 9,
            SpellFlag2   = 10,
            MeleeFlag2   = 11,
            RangedFlag2  = 12,
        }

        public uint UnitId { get; set; }
        public ImmunityType Type { get; set; }
        public uint Value { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Type, 5u);
            writer.Write(Value);
        }
    }
}