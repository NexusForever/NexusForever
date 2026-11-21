using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Story.Message
{
    public class StoryMessage : IWritable
    {
        public uint MsgId { get; set; } // StoryPanelId for message 0x75A otherwise LocalizedTextId
        public uint RandomTextLineId { get; set; }
        public List<Actor> Actors { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MsgId);
            writer.Write(RandomTextLineId);
            writer.Write(Actors.Count, 8u);

            foreach (Actor actor in Actors)
            {
                writer.Write(actor.Type, 3u);
                actor.Write(writer);
            }
        }
    }
}
