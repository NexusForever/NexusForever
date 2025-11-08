using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Story
{
    // Appears over the player's unit
    // In practice the only time this message was sent was to sent LocalizedTextId = 0x5F95C for the text 'Evade'
    [Message(GameMessageOpcode.ServerGenericFloaterComplex)]
    public class ServerGenericFloaterComplex : IWritable
    {
        public uint LocalizedTextId { get; set; }
        public uint RandomTextLineId { get; set; }
        public List<StoryMessage> Messages { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(LocalizedTextId);
            writer.Write(RandomTextLineId);
            writer.Write(Messages.Count, 8u);
            Messages.ForEach(message => message.Write(writer));
        }
    }
}
