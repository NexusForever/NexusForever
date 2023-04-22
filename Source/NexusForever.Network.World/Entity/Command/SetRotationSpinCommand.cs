namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetRotationSpin)]
    public class SetRotationSpinCommand : IEntityCommandModel
    {
        public Position Rotation { get; set; }
        public uint FlightTime { get; set; }
        public ushort Spin { get; set; }
        public uint Offset { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            Rotation = new Position();
            Rotation.Read(reader);

            FlightTime = reader.ReadUInt();
            Spin = reader.ReadUShort();
            Offset = reader.ReadUInt();
            Blend = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            Rotation.Write(writer);
            writer.Write(FlightTime);
            writer.Write(Spin);
            writer.Write(Offset);
            writer.Write(Blend);
        }
    }
}
