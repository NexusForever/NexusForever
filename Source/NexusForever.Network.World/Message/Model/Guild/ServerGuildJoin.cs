using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Guild;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGuildJoin)]
    public class ServerGuildJoin : IWritable
    {
        public GuildData GuildData { get; set; } = new();
        public GuildMember Self { get; set; } = new();
        public GuildWithdrawlInfo BankWithdrawlInfo { get; set; } = new();
        public bool DisplayThisGuildNameplate { get; set; } = true;

        public void Write(GamePacketWriter writer)
        {
            GuildData.Write(writer);
            Self.Write(writer);
            BankWithdrawlInfo.Write(writer);
            writer.Write(DisplayThisGuildNameplate);
        }
    }
}
