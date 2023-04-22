﻿using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.RBAC;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Path, "A collection of commands to manage paths for a character.", "path")]
    [CommandTarget(typeof(IPlayer))]
    public class PathCommandCategory : CommandCategory
    {
        [Command(Permission.PathUnlock, "Unlock a path for character.", "unlock")]
        public void HandlePathUnlock(ICommandContext context,
            [Parameter("Path id to unlock.", ParameterFlags.None, typeof(EnumParameterConverter<Path>))]
            Path path)
        {
            context.GetTargetOrInvoker<IPlayer>().PathManager.UnlockPath(path);
        }

        [Command(Permission.PathActivate, "Activate a path for character.", "activate")]
        public void HandlePathActivate(ICommandContext context,
            [Parameter("Path id to activate.", ParameterFlags.None, typeof(EnumParameterConverter<Path>))]
            Path path)
        {
            context.GetTargetOrInvoker<IPlayer>().PathManager.ActivatePath(path);
        }

        [Command(Permission.PathXP, "Add XP to current path for character.", "xp")]
        public void HandlePathXP(ICommandContext context,
            [Parameter("XP amount to add to currency path.")]
            uint xp)
        {
            context.GetTargetOrInvoker<IPlayer>().PathManager.AddXp(xp);
        }
    }
}
