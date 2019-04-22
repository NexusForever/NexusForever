using System.Collections.Generic;
using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetRotationMultiSpline)]
    public class SetRotationMultiSplineCommand : IEntityCommandModel
    {
        public List<uint> SplineIds { get; set; } = new List<uint>();

        public float Speed { get; set; }
        public float Position { get; set; }
        public float TakeoffLocationHeight { get; set; }
        public float LandingLocationHeight { get; set; }

        public byte Mode { get; set; }
        public uint Offset { get; set; }
        public uint MultiSplineFlags { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUShort(10u);
            for (int i = 0; i < count; i++)
                SplineIds.Add(reader.ReadUInt());

            Speed = reader.ReadPackedFloat();
            Position = reader.ReadUInt();
            TakeoffLocationHeight = reader.ReadUInt();
            LandingLocationHeight = reader.ReadUInt();
            Mode = reader.ReadByte(4u);
            Offset = reader.ReadUInt();
            MultiSplineFlags = reader.ReadUInt();
            Blend = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SplineIds.Count, 10u);
            foreach (uint splineId in SplineIds)
                writer.Write(splineId);

            writer.WritePackedFloat(Speed);
            writer.Write(Position);
            writer.Write(TakeoffLocationHeight);
            writer.Write(LandingLocationHeight);
            writer.Write(Mode, 4);
            writer.Write(Offset);
            writer.Write(MultiSplineFlags);
            writer.Write(Blend);
        }
    }
}
