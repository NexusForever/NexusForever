using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Story
{
    // Appears over the player's unit
    [Message(GameMessageOpcode.ServerGenericFloaterLocalised)]
    public class ServerGenericFloaterLocalised : IWritable
    {
        public uint LocalisedTextId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(LocalisedTextId);
        }
    }
}
