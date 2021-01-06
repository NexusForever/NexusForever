using NexusForever.WorldServer.Game.Guild.Static;

namespace NexusForever.WorldServer.Game.Guild
{
    public class GuildResultInfo
    {
        public GuildResult Result { get; set; }
        public ulong GuildId { get; set; }
        public string ReferenceString { get; set; }
        public uint ReferenceId { get; set; }

        public GuildResultInfo(GuildResult result, ulong guildId = 0ul, string referenceString = "", uint referenceId = 0u)
        {
            Result          = result;
            GuildId         = guildId;
            ReferenceString = referenceString;
            ReferenceId     = referenceId;
        }
    }
}
