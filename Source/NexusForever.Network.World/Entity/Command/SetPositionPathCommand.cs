using System.Numerics;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetPositionPath)]
    public class SetPositionPathCommand : IEntityCommandModel
    {
        public List<Vector3> Positions { get; set; } = new();
        public float Speed { get; set; }
        public SplineType Type { get; set; }
        public SplineMode Mode { get; set; }
        public uint Offset { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUShort(10u);
            for (int i = 0; i < count; i++)
                Positions.Add(reader.ReadVector3());

            Speed  = reader.ReadPackedFloat();
            Type   = reader.ReadEnum<SplineType>(2u);
            Mode   = reader.ReadEnum<SplineMode>(4u);
            Offset = reader.ReadUInt();
            Blend  = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Positions.Count, 10u);
            Positions.ForEach(writer.WriteVector3);

            writer.WritePackedFloat(Speed);
            writer.Write(Type, 2u);
            writer.Write(Mode, 4u);
            writer.Write(Offset);
            writer.Write(Blend);
        }
    }
}
