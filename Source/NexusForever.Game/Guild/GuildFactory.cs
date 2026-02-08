using Microsoft.Extensions.DependencyInjection;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Static.Guild;

namespace NexusForever.Game.Guild
{
    public class GuildFactory : IGuildFactory
    {
        #region Dependency Injection

        private readonly IServiceProvider serviceProvider;

        public GuildFactory(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        #endregion

        public IGuildBase CreateGuild(GuildModel model)
        {
            IGuildBase guildBase = serviceProvider.GetKeyedService<IGuildBase>((GuildType)model.Type);
            guildBase.Initialise(model);
            return guildBase;
        }

        public IGuildBase CreateGuild(GuildType type, string guildName, string leaderRankName, string councilRankName, string memberRankName, IGuildStandard standard = null)
        {
            IGuildBase guildBase = serviceProvider.GetKeyedService<IGuildBase>(type);

            switch (guildBase)
            {
                case IGuild guild:
                    guild.Initialise(guildName, leaderRankName, councilRankName, memberRankName, standard);
                    break;
                case IArenaTeam arenaTeam:
                    arenaTeam.Initialise(type, guildName, leaderRankName, councilRankName, memberRankName);
                    break;
                default:
                    guildBase.Initialise(guildName, leaderRankName, councilRankName, memberRankName);
                    break;
            }

            return guildBase;
        }
    }
}
