using NexusForever.Game.Static.Story;

namespace NexusForever.Network.World.Message.Model.Story.Message
{
    public sealed class LocalisedTextActor : Actor
    {
        public override StoryTextSourceType Type => StoryTextSourceType.LocalizedText;
        public uint LocalisedTextId { get; set; }

        public override void Write(GamePacketWriter writer)
        {
            writer.Write(LocalisedTextId, 21u);
            base.Write(writer);
        }
    }
}
