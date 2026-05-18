using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerTitles)]
    public class ServerTitles : IWritable
    {
        public class Title : IWritable
        {
            public ushort TitleId { get; set; }
            public bool Revoked { get; set; }
            public bool InSchedule { get; set; }
            public uint TimeRemaining { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(TitleId, 14u);
                writer.Write(Revoked);
                writer.Write(InSchedule);
                writer.Write(TimeRemaining);
            }
        }

        public List<Title> Titles { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Titles.Count);
            Titles.ForEach(t => t.Write(writer));
        }
    }
}
