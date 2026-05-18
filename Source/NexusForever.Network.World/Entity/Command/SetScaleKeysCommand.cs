using NexusForever.Game.Static.Entity.Movement.Command;

namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetScaleKeys)]
    public class SetScaleKeysCommand : IEntityCommandModel
    {
        public List<uint> Times { get; set; } = new();
        public List<float> Scales { get; set; } = new();
        public byte Type { get; set; }
        public uint Offset { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            byte count = reader.ReadByte();

            for (int i = 0; i < count; i++)
                Times.Add(reader.ReadUInt());

            for (int i = 0; i < count; i++)
                Scales.Add(reader.ReadPackedFloat());

            Type   = reader.ReadByte(2u);
            Offset = reader.ReadUInt();
            Blend  = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Times.Count, 8u);
            foreach (uint time in Times)
                writer.Write(time);

            foreach (float scale in Scales)
                writer.WritePackedFloat(scale);

            writer.Write(Type, 2u);
            writer.Write(Offset);
            writer.Write(Blend);
        }
    }
}
