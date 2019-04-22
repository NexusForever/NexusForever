using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Game.Entity.Movement.Spline.Static;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetPositionPath)]
    public class SetPositionPathCommand : IEntityCommandModel
    {
        public List<Position> Positions { get; set; } = new List<Position>();
        public float Speed { get; set; }
        public SplineType Type { get; set; }
        public SplineMode Mode { get; set; }
        public uint Offset { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUShort(10u);
            for (int i = 0; i < count; i++)
            {
                Position position = new Position();
                position.Read(reader);
                Positions.Add(position);
            }

            Speed  = reader.ReadPackedFloat();
            Type   = reader.ReadEnum<SplineType>(2u);
            Mode   = reader.ReadEnum<SplineMode>(4u);
            Offset = reader.ReadUInt();
            Blend  = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Positions.Count, 10u);
            Positions.ForEach(p => p.Write(writer));

            writer.WritePackedFloat(Speed);
            writer.Write(Type, 2u);
            writer.Write(Mode, 4u);
            writer.Write(Offset);
            writer.Write(Blend);
        }
    }
}
