using System.Collections.Generic;

namespace NexusForever.Shared.Network.Message.Model.Shared
{
    public class Message : IWritable
    {
        public uint Index { get; set; }
        public List<string> Messages { get; set; } = new List<string>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Index);
            writer.Write(Messages.Count, 8u);
            Messages.ForEach(writer.WriteStringWide);
        }
    }
}
