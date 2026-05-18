using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Guild;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.RBAC;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Guild, "A collection of commands to manage a guilds.", "guild")]
    [CommandTarget(typeof(IPlayer))]
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
            IPlayer player = context.Invoker as IPlayer;

            // default ranks from client
            leaderRank  ??= "Leader";
            councilRank ??= "Council";
            memberRank  ??= "Member";

            // default standard from the client
            IGuildStandard standard = null;
            if (type == GuildType.Guild)
                standard = new GuildStandard(4, 5, 6);

            IGuildResultInfo info = player.GuildManager.CanRegisterGuild(type, name, leaderRank, councilRank, memberRank, standard);
            if (info.Result != GuildResult.Success)
            {
                GuildBase.SendGuildResult(player.Session, info);
                return;
            }

            player.GuildManager.RegisterGuild(type, name, leaderRank, councilRank, memberRank, standard);
        }

        [Command(Permission.GuildJoin, "Join an existing guild.", "join")]
        public void HandleGuildJoin(ICommandContext context,
            [Parameter("Type of guild to join.", ParameterFlags.None, typeof(EnumParameterConverter<GuildType>))]
            GuildType type,
            [Parameter("Name of guild to join.")]
            string name)
        {
            IPlayer player = context.Invoker as IPlayer;

            ulong guildId = GlobalGuildManager.Instance.GetGuild(type, name)?.Id ?? 0;

            IGuildResultInfo info = player.GuildManager.CanJoinGuild(guildId);
            if (info.Result != GuildResult.Success)
            {
                GuildBase.SendGuildResult(player.Session, info);
                return;
            }

            player.GuildManager.JoinGuild(guildId);
        }
    }
}
