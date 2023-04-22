﻿using NexusForever.Game.Abstract.Account.Unlock;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.RBAC;
using NexusForever.GameTable;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Generic, "A collection of commands to manage generic unlocks for an account.", "generic")]
    [CommandTarget(typeof(IPlayer))]
    public class GenericUnlockCommandCategory : CommandCategory
    {
        [Command(Permission.GenericUnlock, "Unlock generic unlock entry for an account.", "unlock")]
        public void HandleGenericUnlockUnlock(ICommandContext context,
            [Parameter("Generic unlock entry to unlock.")]
            ushort genericUnlockEntryId)
        {
            context.GetTargetOrInvoker<IPlayer>().Account.GenericUnlockManager.Unlock(genericUnlockEntryId);
        }

        [Command(Permission.GenericUnlockAll, "Unlock all generic unlocks of type for an account.", "unlockall")]
        public void HandleGenericUnlockUnlockAll(ICommandContext context,
            [Parameter("Generic unlock type to unlock all entries from.", ParameterFlags.None, typeof(EnumParameterConverter<GenericUnlockType>))]
            GenericUnlockType genericUnlockType)
        {
            context.GetTargetOrInvoker<IPlayer>().Account.GenericUnlockManager.UnlockAll(genericUnlockType);
        }

        [Command(Permission.GenericList, "ist all acquired generic unlock entries for an account.", "list")]
        public void HandleGenericUnlockList(ICommandContext context)
        {
            context.SendMessage("Acquired generic unlock entries:");

            TextTable tt = GameTableManager.Instance.GetTextTable(context.Language);
            foreach (IGenericUnlock unlock in context.GetTargetOrInvoker<IPlayer>().Account.GenericUnlockManager)
            {
                string name = tt.GetEntry(unlock.Entry.LocalizedTextIdDescription);
                context.SendMessage($"{unlock.Entry.Id}, {unlock.Type}, {name}");
            }
        }
    }
}
