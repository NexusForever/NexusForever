using System;
using NexusForever.Game.Static.RBAC;
using NexusForever.Script;
using NexusForever.Script.Static;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Script, "A collection of commands to manage the script system.", "script")]
    public class ScriptCommandCategory : CommandCategory
    {
        [Command(Permission.ScriptReload, "Reload a script assembly.", "reload")]
        public void HandleScriptReload(
            ICommandContext context,
            [Parameter("Assembly name to reload.")]
            string assemblyName,
            [Parameter("Type of assembly reload to perform.", converter: typeof(EnumParameterConverter<ReloadType>))]
            ReloadType reloadType)
        {
            DateTime start = DateTime.UtcNow;
            ScriptManager.Instance.Reload(assemblyName, reloadType);
            context.SendMessage($"{assemblyName} reloaded in {(DateTime.UtcNow - start).TotalMilliseconds}ms.");
        }

        [Command(Permission.ScriptInfo, "", "info")]
        public void HandleScriptInfo(
            ICommandContext context)
        {
            context.SendMessage(ScriptManager.Instance.Information());
        }
    }
}
