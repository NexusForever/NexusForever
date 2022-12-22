using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerChat)]
    public class ServerChat : IWritable
    {
        public Channel Channel { get; set; }
        public bool GM { get; set; }
        public bool Self { get; set; }
        public bool AutoResponse { get; set; }
        public TargetPlayerIdentity From { get; set; } = new();
        public string FromName { get; set; }
        public string FromRealm { get; set; }
        public ChatPresenceState PresenceState { get; set; }

        public string Text { get; set; }
        public List<ChatFormat> Formats { get; set; } = new();
        public bool CrossFaction { get; set; }

        public ulong Guid { get; set; }
        public byte PremiumTier { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Channel.Write(writer);

            writer.Write(GM);
            writer.Write(Self);
            writer.Write(AutoResponse);

            From.Write(writer);
            writer.WriteStringWide(FromName);
            writer.WriteStringWide(FromRealm);

            writer.Write(PresenceState, 3u);

            writer.WriteStringWide(Text);
            writer.Write(Formats.Count, 5u);
            Formats.ForEach(f => f.Write(writer));
            writer.Write(CrossFaction);
            writer.Write(0, 16u);

            writer.Write(Guid, 32u);
            writer.Write(PremiumTier);
        }
    }
}
