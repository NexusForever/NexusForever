using System.Numerics;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    // Fires when the current player is revived by another player.
    [Message(GameMessageOpcode.ServerCasterResurrectedPlayer)]
    public class ServerCasterResurrectedPlayer : IWritable
    {
        public uint UnitId { get; set; } // UnitId of the resurrection caster
        public uint SpellId { get; set; } // unused
        public uint TimeUntilRez { get; set; } // unused
        public float PercentageHealthRestored { get; set; } // unused
        public float PercentageEnergyRestored { get; set; } // unused
        public Vector3 Position { get; set; } // unused
        public bool SpellCastOnDead { get; set; } // unused

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(SpellId, 18u);
            writer.Write(TimeUntilRez);
            writer.Write(PercentageHealthRestored);
            writer.Write(PercentageEnergyRestored);
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Position.Z);
            writer.Write(false);
        }
    }
}
