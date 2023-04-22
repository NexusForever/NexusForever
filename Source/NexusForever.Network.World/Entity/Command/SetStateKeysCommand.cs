namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetStateKeys)]
    public class SetStateKeysCommand : IEntityCommandModel
    {
        public List<uint> Times = new();
        public List<uint> States = new();

        public uint Offset { get; set; }

        public void Read(GamePacketReader reader)
        {
            uint Count = reader.ReadByte();

            for (int i = 0; i < Count; i++)
                Times.Add(reader.ReadUInt());

            for (int i = 0; i < Count; i++)
                States.Add(reader.ReadUInt());

            Offset = reader.ReadUInt();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Times.Count, 8u);

            foreach (var time in Times)
                writer.Write(time);

            foreach (var state in States)
                writer.Write(state);

            writer.Write(Offset);
        }
    }
}
