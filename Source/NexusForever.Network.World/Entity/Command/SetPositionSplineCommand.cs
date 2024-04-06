using System.Numerics;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetPositionSpline)]
    public class SetPositionSplineCommand : IEntityCommandModel
    {
        public uint SplineId { get; set; }
        public float Speed { get; set; }
        public float Position { get; set; }
        public Vector3 FormationData { get; set; }
        public SplineMode Mode { get; set; }
        public uint Offset { get; set; }
        public bool Blend { get; set; }
        public bool IsContinuing { get; set; }
        public bool AdjustSpeedToLength { get; set; }

        public void Read(GamePacketReader reader)
        {
            SplineId            = reader.ReadUInt();
            Speed               = reader.ReadSingle();
            Position            = reader.ReadSingle();
            FormationData       = reader.ReadPackedVector3();
            Mode                = reader.ReadEnum<SplineMode>(4u);
            Offset              = reader.ReadUInt();
            Blend               = reader.ReadBit();
            IsContinuing        = reader.ReadBit();
            AdjustSpeedToLength = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SplineId);
            writer.Write(Speed);
            writer.Write(Position);
            writer.WritePackedVector3(FormationData);
            writer.Write(Mode, 4u);
            writer.Write(Offset);
            writer.Write(Blend);
            writer.Write(IsContinuing);
            writer.Write(AdjustSpeedToLength);
        }
    }
}
