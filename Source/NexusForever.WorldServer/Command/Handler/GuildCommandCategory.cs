using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Guild;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Game.RBAC.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Guild, "A collection of commands to manage a guilds.", "guild")]
    [CommandTarget(typeof(Player))]
    public class GuildCommandCategory : CommandCategory
    {
        [Command(Permission.GuildRegister, "Register a new guild.", "register")]
        public void HandleGuildRegister(ICommandContext context,
            [Parameter("Guild type to create.", ParameterFlags.None, typeof(EnumParameterConverter<GuildType>))]
            GuildType type,
            [Parameter("Name of newly created guild.")]
            string name,
            [Parameter("", ParameterFlags.Optional)]
            string leaderRank,
            [Parameter("", ParameterFlags.Optional)]
            string councilRank,
            [Parameter("", ParameterFlags.Optional)]
            string memberRank)
        {
            Player player = context.Invoker as Player;

            // default ranks from client
            leaderRank  ??= "Leader";
            councilRank ??= "Council";
            memberRank  ??= "Member";

            // default standard from the client
            GuildStandard standard = null;
            if (type == GuildType.Guild)
                standard = new GuildStandard(3, 4, 6);

            GuildResultInfo info = player.GuildManager.CanRegisterGuild(type, name, leaderRank, councilRank, memberRank, standard);
            if (info.Result != GuildResult.Success)
            {
                GuildBase.SendGuildResult(player.Session, info);
                return;
            }

            player.GuildManager.RegisterGuild(type, name, leaderRank, councilRank, memberRank, standard);
        }

        [Command(Permission.GuildJoin, "Join an existing guild.", "join")]
        public void HandleGuildJoin(ICommandContext context,
            [Parameter("Name of guild to join.")]
            string name)
        {
            Player player = context.Invoker as Player;
            ulong guildId = GlobalGuildManager.Instance.GetGuild(name)?.Id ?? 0;

            GuildResultInfo info = player.GuildManager.CanJoinGuild(guildId);
            if (info.Result != GuildResult.Success)
            {
                GuildBase.SendGuildResult(player.Session, info);
                return;
            }

            player.GuildManager.JoinGuild(guildId);
        }
    }
}
