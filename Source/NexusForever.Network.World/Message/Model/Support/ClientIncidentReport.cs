using NexusForever.Game.Static.Support;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Support
{
    [Message(GameMessageOpcode.ClientIncidentReport)]
    public class ClientIncidentReport : IReadable
    {
        public Identity Identity { get; private set; } = new();
        public ReportPlayerReason Reason { get; private set; }
        public ReportPlayerSource Source { get; private set; }
        public string Note { get; private set; }
        public ulong ObjectId { get; private set; } // mailId, guildInviteId, friendshipInviteId, friendAccountInviteId, unitId
        public float DaysAgo { get; private set; }
        public bool PermanentIgnore { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Identity.Read(reader);
            Reason = reader.ReadEnum<ReportPlayerReason>(3u);
            Source = reader.ReadEnum<ReportPlayerSource>(4u);
            Note = reader.ReadWideString();
            ObjectId = reader.ReadULong();
            DaysAgo = reader.ReadSingle();
            PermanentIgnore = reader.ReadBit();
        }
    }
}
