using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Static.Guild;

namespace NexusForever.Game.Guild
{
    public class GuildResultInfo : IGuildResultInfo
    {
        public GuildResult Result { get; set; }
        public Identity GuildIdentity { get; set; }
        public string ReferenceString { get; set; }
        public uint ReferenceId { get; set; }

        public GuildResultInfo(GuildResult result, Identity guildIdentity, string referenceString = "", uint referenceId = 0u)
        {
            Result          = result;
            GuildIdentity   = guildIdentity;
            ReferenceString = referenceString;
            ReferenceId     = referenceId;
        }

        public GuildResultInfo(GuildResult result, string referenceString = "", uint referenceId = 0u)
        {
            Result          = result;
            GuildIdentity   = new Identity { Id = 0, RealmId = 0 };
            ReferenceString = referenceString;
            ReferenceId     = referenceId;
        }
    }
}
