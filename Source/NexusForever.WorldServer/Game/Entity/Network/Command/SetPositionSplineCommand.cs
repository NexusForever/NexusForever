using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetPositionSpline)]
    public class SetPositionSplineCommand : IEntityCommand
    {
        public uint SplineId { get; set; }
        public float Speed { get; set; }
        public uint Position { get; set; }
        public Formation FormationData { get; set; }
        public byte Mode { get; set; }
        public uint Offset { get; set; }
        public bool Blend { get; set; }
        public bool IsContinuing { get; set; }
        public bool AdjustSpeedToLength { get; set; }

        public void Read(GamePacketReader reader)
        {
            FormationData = new Formation();

            SplineId= reader.ReadUInt();
            Speed   = reader.ReadUInt();
            Position = reader.ReadUInt();
            FormationData.Read(reader);
            Mode    = reader.ReadByte(4u);
            Offset  = reader.ReadUInt();
            Blend   = reader.ReadBit();
            IsContinuing = reader.ReadBit();
            AdjustSpeedToLength = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SplineId);
            writer.Write(Speed);
            writer.Write(Position);
            FormationData.Write(writer);
            writer.Write(Mode, 4u);
            writer.Write(Offset);
            writer.Write(Blend);
            writer.Write(IsContinuing);
            writer.Write(AdjustSpeedToLength);
        }
    }
}
