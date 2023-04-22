namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetModeKeys)]
    public class SetModeKeysCommand : IEntityCommandModel
    {
        public List<uint> Times = new();
        public List<uint> Modes = new();

        public uint Offset { get; set; }

        public void Read(GamePacketReader reader)
        {
            uint Count = reader.ReadByte();

            for (int i = 0; i < Count; i++)
                Times.Add(reader.ReadUInt());

            for (int i = 0; i < Count; i++)
                Modes.Add(reader.ReadUInt());

            Offset = reader.ReadUInt();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Times.Count, 8u);

            foreach (var u in Times)
                writer.Write(u);

            foreach (var u in Modes)
                writer.Write(u);

            writer.Write(Offset);
        }
    }
}
