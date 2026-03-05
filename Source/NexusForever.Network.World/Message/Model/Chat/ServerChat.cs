using NexusForever.Game.Static.Chat;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Chat
{
    [Message(GameMessageOpcode.ServerChat)]
    public class ServerChat : IWritable
    {
        public Channel Channel { get; set; }
        public bool GM { get; set; }
        public bool Self { get; set; }
        public bool AutoResponse { get; set; }
        public Identity From { get; set; }
        public string FromName { get; set; }
        public string FromRealm { get; set; }
        public AccountPresenceState PresenceState { get; set; }

        public string Text { get; set; }
        public List<ChatFormat> Formats { get; set; } = [];
        public bool CrossFaction { get; set; }
        public ushort ChatMessageId { get; set; } = 0; // always 0

        public ulong UnitId { get; set; }
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
            writer.Write(ChatMessageId, 16u);

            writer.Write(UnitId, 32u);
            writer.Write(PremiumTier);
        }
    }
}
