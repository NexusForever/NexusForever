using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Guild.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerGuildInvite)]
    public class ServerGuildInvite : IWritable
    {
        public string PlayerName { get; set; }
        public string GuildName { get; set; }
        public uint Flags { get; set; }
        public GuildType GuildType { get; set; }
        public ulong Unknown4 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.WriteStringWide(PlayerName);
            writer.WriteStringWide(GuildName);
            writer.Write(Flags);
            writer.Write(GuildType, 4u);
            writer.Write(Unknown4);
        }
    }
}
