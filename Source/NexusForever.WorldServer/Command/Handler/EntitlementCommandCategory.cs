using NexusForever.Game.Abstract.Account.Entitlement;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.RBAC;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Entitlement, "A collection of commands to manage account and character entitlements.", "entitlement")]
    [CommandTarget(typeof(IPlayer))]
    public class EntitlementCommandCategory : CommandCategory
    {
        [Command(Permission.EntitlementAccount, "A collection of commands to manage account entitlements", "account")]
        public class EntitlementCommandAccountCategory : CommandCategory
        {
            [Command(Permission.EntitlementAccountList, "List all entitlements for character.", "list")]
            public void HandleEntitlementCommandAccountList(ICommandContext context)
            {
                IPlayer player = context.GetTargetOrInvoker<IPlayer>();
                context.SendMessage($"Entitlements for account {player.Account.Id}:");
                foreach (IAccountEntitlement entitlement in player.Account.EntitlementManager) 
                    context.SendMessage($"Entitlement: {entitlement.Type}, Value: {entitlement.Amount}");
            }
        }

        [Command(Permission.EntitlementCharacter, "A collection of commands to manage character entitlements", "character")]
        public class EntitlementCommandCharacterCategory : CommandCategory
        {
            [Command(Permission.EntitlementCharacterList, "List all entitlements for account.", "list")]
            public void HandleEntitlementCommandCharacterList(ICommandContext context)
            {
                IPlayer player = context.GetTargetOrInvoker<IPlayer>();
                context.SendMessage($"Entitlements for character {player.CharacterId}:");
                foreach (ICharacterEntitlement entitlement in player.EntitlementManager)
                    context.SendMessage($"Entitlement: {entitlement.Type}, Value: {entitlement.Amount}");
            }
        }

        [Command(Permission.EntitlementAdd, "Create or update an entitlement.", "add")]
        public void HandleEntitlementCommandAdd(ICommandContext context,
                [Parameter("Entitlement type to modify.", ParameterFlags.None, typeof(EnumParameterConverter<EntitlementType>))]
                EntitlementType entitlementType,
                [Parameter("Value to modify the entitlement.")]
                int value)
        {
            EntitlementEntry entry = GameTableManager.Instance.Entitlement.GetEntry((ulong)entitlementType);
            if (entry == null)
            {
                context.SendMessage($"{entitlementType} isn't a valid entitlement id!");
                return;
            }

            IPlayer targetPlayer = context.GetTargetOrInvoker<IPlayer>();
            if (targetPlayer != context.Invoker && !(context.Invoker as IPlayer).Account.RbacManager.HasPermission(Permission.EntitlementGrantOther))
                targetPlayer = context.Invoker as IPlayer;

            if (((EntitlementFlags)entry.Flags & EntitlementFlags.Character) != 0)
                targetPlayer.EntitlementManager.UpdateEntitlement(entitlementType, value);
            else
                targetPlayer.Account.EntitlementManager.UpdateEntitlement(entitlementType, value);            
        }
    }
}
