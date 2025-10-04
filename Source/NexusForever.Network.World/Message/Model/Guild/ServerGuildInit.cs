using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildInit)]
    public class ServerGuildInit : IWritable
    {
        public uint ShowNameplateIndex { get; set; } // Of the guilds in the Guilds, this is the list index of the guild the player will display on their nameplate
        public List<GuildData> Guilds { get; set; } = new();
        public List<GuildMember> Self { get; set; } = new();
        public List<GuildWithdrawlInfo> BankWithdrawlInfo { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guilds.Count);
            writer.Write(ShowNameplateIndex);
            Guilds.ForEach(w => w.Write(writer));
            Self.ForEach(w => w.Write(writer));
            BankWithdrawlInfo.ForEach(w => w.Write(writer));
        }
    }
}
