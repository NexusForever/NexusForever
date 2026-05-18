using System.Numerics;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetPositionMultiSpline)]
    public class SetPositionMultiSplineCommand : IEntityCommandModel
    {
        public List<uint> SplineIds { get; set; } = new();
        public float Speed { get; set; }
        public float Position { get; set; }
        public float TakeoffLocationHeight { get; set; }
        public float LandingLocationHeight { get; set; }
        public Vector3 FormationData { get; set; }
        public SplineMode Mode { get; set; }
        public uint Offset { get; set; }
        public uint MultiSplineFlags { get; set; }
        public bool Blend { get; set; }
        public bool IsContinuing { get; set; }

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUShort(10u);

            for (int i = 0; i < count; i++)
                SplineIds.Add(reader.ReadUInt());

            Speed                 = reader.ReadPackedFloat();
            Position              = reader.ReadUInt();
            TakeoffLocationHeight = reader.ReadUInt();
            LandingLocationHeight = reader.ReadUInt();
            FormationData         = reader.ReadPackedVector3();
            Mode                  = reader.ReadEnum<SplineMode>(4u);
            Offset                = reader.ReadUInt();
            MultiSplineFlags      = reader.ReadUInt();
            Blend                 = reader.ReadBit();
            IsContinuing          = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SplineIds.Count, 10u);
            foreach (var splineId in SplineIds)
                writer.Write(splineId);

            writer.Write(Speed);
            writer.Write(Position);
            writer.Write(TakeoffLocationHeight);
            writer.Write(LandingLocationHeight);
            writer.WritePackedVector3(FormationData);
            writer.Write(Mode, 4);
            writer.Write(Offset);
            writer.Write(MultiSplineFlags);
            writer.Write(Blend);
            writer.Write(IsContinuing);
        }
    }
}
