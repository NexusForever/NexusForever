using NexusForever.Shared.GameTable;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.RBAC.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Generic, "A collection of commands to manage generic unlocks for an account.", "generic")]
    [CommandTarget(typeof(Player))]
    public class GenericUnlockCommandCategory : CommandCategory
    {
        [Command(Permission.GenericUnlock, "Unlock generic unlock entry for an account.", "unlock")]
        public void HandleGenericUnlockUnlock(ICommandContext context,
            [Parameter("Generic unlock entry to unlock.")]
            ushort genericUnlockEntryId)
        {
            context.GetTargetOrInvoker<Player>().Session.GenericUnlockManager.Unlock(genericUnlockEntryId);
        }

        [Command(Permission.GenericUnlockAll, "Unlock all generic unlocks of type for an account.", "unlockall")]
        public void HandleGenericUnlockUnlockAll(ICommandContext context,
            [Parameter("Generic unlock type to unlock all entries from.", ParameterFlags.None, typeof(EnumParameterConverter<GenericUnlockType>))]
            GenericUnlockType genericUnlockType)
        {
            context.GetTargetOrInvoker<Player>().Session.GenericUnlockManager.UnlockAll(genericUnlockType);
        }

        [Command(Permission.GenericList, "ist all acquired generic unlock entries for an account.", "list")]
        public void HandleGenericUnlockList(ICommandContext context)
        {
            context.SendMessage("Acquired generic unlock entries:");

            TextTable tt = GameTableManager.Instance.GetTextTable(context.Language);
            foreach (GenericUnlock unlock in context.GetTargetOrInvoker<Player>().Session.GenericUnlockManager)
            {
                string name = tt.GetEntry(unlock.Entry.LocalizedTextIdDescription);
                context.SendMessage($"{unlock.Entry.Id}, {unlock.Type}, {name}");
            }
        }
    }
}
