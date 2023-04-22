namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetScaleKeys)]
    public class SetScaleKeysCommand : IEntityCommandModel
    {
        public List<uint> Times = new();
        public byte Type { get; set; }
        public uint Offset { get; set; }
        public bool Blend { get; set; }
        public List<ushort> Scales = new();

        public void Read(GamePacketReader reader)
        {
            byte Count = reader.ReadByte();

            for (int i = 0; i < Count; i++)
                Times.Add(reader.ReadUInt());

            Type = reader.ReadByte(2u);
            Offset = reader.ReadUInt();
            Blend = reader.ReadBit();

            for (int i = 0; i < Count; i++)
                Scales.Add(reader.ReadUShort());

        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Times.Count, 8u);
            foreach (var time in Times)
                writer.Write(time);

            writer.Write(Type, 2u);
            writer.Write(Offset);
            writer.Write(Blend);

            foreach (var scale in Scales)
                writer.Write(scale);
        }
    }
}
