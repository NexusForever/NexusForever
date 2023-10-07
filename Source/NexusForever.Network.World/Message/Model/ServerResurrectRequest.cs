using System.Numerics;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerResurrectRequest)]
    public class ServerResurrectRequest : IWritable
    {
        public uint UnitId { get; set; }
        public uint SpellId { get; set; }
        public uint TimeUntilRez { get; set; }
        public float PercentageHealthRestored { get; set; }
        public float PercentageEnergyRestored { get; set; }
        public Vector3 Position { get; set; }
        public bool SpellCastOnDead { get; set; }

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
