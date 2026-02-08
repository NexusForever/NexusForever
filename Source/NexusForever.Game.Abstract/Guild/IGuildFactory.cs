using NexusForever.Database.Character.Model;
using NexusForever.Game.Static.Guild;

namespace NexusForever.Game.Abstract.Guild
{
    public interface IGuildFactory
    {
        IGuildBase CreateGuild(GuildModel model);

        IGuildBase CreateGuild(GuildType type, string guildName, string leaderRankName, string councilRankName, string memberRankName, IGuildStandard standard = null);
    }
}
