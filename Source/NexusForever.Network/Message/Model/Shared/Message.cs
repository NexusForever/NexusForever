namespace NexusForever.Network.Message.Model.Shared
{
    public class Message : IWritable
    {
        public uint Index { get; set; }
        public List<string> Messages { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Index);
            writer.Write(Messages.Count, 8u);
            Messages.ForEach(writer.WriteStringWide);
        }
    }
}
