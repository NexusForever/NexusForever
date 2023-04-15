using NexusForever.Game.Static.Guild;

namespace NexusForever.Game.Abstract.Guild
{
    public interface IGuildResultInfo
    {
        GuildResult Result { get; set; }
        ulong GuildId { get; set; }
        string ReferenceString { get; set; }
        uint ReferenceId { get; set; }
    }
}