using NexusForever.Game.Static.Guild;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGuildResult)]
    public class ServerGuildResult : IWritable
    {
        public ushort RealmId { get; set; }
        public ulong GuildId { get; set; }

        /// <summary>
        /// This is used as an ID reference, For example, for Rank index
        /// </summary>
        public uint ReferenceId { get; set; }

        public string ReferenceText { get; set; }
        public GuildResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RealmId, 14u);
            writer.Write(GuildId);
            writer.Write(ReferenceId);
            writer.WriteStringWide(ReferenceText);
            writer.Write(Result, 8u);
        }
    }
}
