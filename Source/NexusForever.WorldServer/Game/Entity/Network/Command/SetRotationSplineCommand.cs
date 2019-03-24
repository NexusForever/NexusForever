using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetRotationSpline)]
    public class SetRotationSplineCommand : IEntityCommand
    {
        public uint SplineId { get; set; }
        public ushort Speed { get; set; }
        public float Position { get; set; }
        public byte Mode { get; set; }
        public uint Offset { get; set; }
        public bool Blend { get; set; }
        public bool AdjustSpeedToLength { get; set; }

        public void Read(GamePacketReader reader)
        {
            SplineId = reader.ReadUInt();
            Speed = reader.ReadUShort();
            Position = reader.ReadUInt();
            Mode = reader.ReadByte(4u);
            Offset = reader.ReadUInt();
            Blend = reader.ReadBit();
            AdjustSpeedToLength = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SplineId);
            writer.Write(Speed);
            writer.Write(Position);
            writer.Write(Mode, 4u);
            writer.Write(Offset);
            writer.Write(Blend);
            writer.Write(AdjustSpeedToLength);
        }
    }
}
