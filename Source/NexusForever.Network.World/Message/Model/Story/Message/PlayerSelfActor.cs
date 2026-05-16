using NexusForever.Game.Static.Story;

namespace NexusForever.Network.World.Message.Model.Story.Message
{
    public sealed class PlayerSelfActor : Actor
    {
        public override StoryTextSourceType Type => StoryTextSourceType.PlayerSelf;
        public uint PlayerUnitId { get; set; } // not used by the client

        public override void Write(GamePacketWriter writer)
        {
            writer.Write(PlayerUnitId);
            base.Write(writer);
        }
    }
}
