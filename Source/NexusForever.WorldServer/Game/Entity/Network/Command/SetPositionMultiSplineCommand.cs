using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetPositionMultiSpline)]
    public class SetPositionMultiSplineCommand : IEntityCommandModel
    {
        public List<uint> SplineIds = new List<uint>();
        public ushort Speed { get; set; }
        public float Position { get; set; }
        public float TakeoffLocationHeight { get; set; }
        public float LandingLocationHeight { get; set; }
        public Formation FormationData { get; set; }
        public byte Mode { get; set; }
        public uint Offset { get; set; }
        public uint MultiSplineFlags { get; set; }
        public bool Blend { get; set; }
        public bool IsContinuing { get; set; }

        public void Read(GamePacketReader reader)
        {
            uint Count = reader.ReadUShort(10u);
            FormationData = new Formation();

            for (int i = 0; i < Count; i++)
                SplineIds.Add(reader.ReadUInt());

            Speed = reader.ReadUShort();
            Position = reader.ReadUInt();
            TakeoffLocationHeight = reader.ReadUInt();
            LandingLocationHeight = reader.ReadUInt();
            FormationData.Read(reader);
            Mode = reader.ReadByte(4u);
            Offset = reader.ReadUInt();
            MultiSplineFlags = reader.ReadUInt();
            Blend = reader.ReadBit();
            IsContinuing = reader.ReadBit();
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
            FormationData.Write(writer);
            writer.Write(Mode, 4);
            writer.Write(Offset);
            writer.Write(MultiSplineFlags);
            writer.Write(Blend);
            writer.Write(IsContinuing);
        }
    }
}
