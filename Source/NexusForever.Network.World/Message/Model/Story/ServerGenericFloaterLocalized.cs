using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Story
{
    // Appears over the player's unit
    [Message(GameMessageOpcode.ServerGenericFloaterLocalized)]
    public class ServerGenericFloaterLocalized : IWritable
    {
        public uint LocalizedStringId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(LocalizedStringId);
        }
    }
}
