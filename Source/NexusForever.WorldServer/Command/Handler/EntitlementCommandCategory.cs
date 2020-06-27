using NexusForever.Shared.GameTable;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.RBAC.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Entitlement, "A collection of commands to manage account and character entitlements.", "entitlement")]
    [CommandTarget(typeof(Player))]
    public class EntitlementCommandCategory : CommandCategory
    {
        [Command(Permission.EntitlementAccount, "A collection of commands to manage account entitlements", "account")]
        public class EntitlementCommandAccountCategory : CommandCategory
        {
            [Command(Permission.EntitlementAccountAdd, "Create or update an account entitlement.", "add")]
            public void HandleEntitlementCommandAccountAdd(ICommandContext context,
                [Parameter("Entitlement type to modify.", ParameterFlags.None, typeof(EnumParameterConverter<EntitlementType>))]
                EntitlementType entitlementType,
                [Parameter("Value to modify the entitlement.")]
                int value)
            {
                if (GameTableManager.Instance.Entitlement.GetEntry((ulong)entitlementType) == null)
                {
                    context.SendMessage($"{entitlementType} isn't a valid entitlement id!");
                    return;
                }

                context.GetTargetOrInvoker<Player>().Session.EntitlementManager.SetAccountEntitlement(entitlementType, value);
            }

            [Command(Permission.EntitlementAccountList, "List all entitlements for character.", "list")]
            public void HandleEntitlementCommandAccountList(ICommandContext context)
            {
                Player player = context.GetTargetOrInvoker<Player>();
                context.SendMessage($"Entitlements for account {player.Session.Account.Id}:");
                foreach (AccountEntitlement entitlement in player.Session.EntitlementManager.GetAccountEntitlements()) 
                    context.SendMessage($"Entitlement: {entitlement.Type}, Value: {entitlement.Amount}");
            }
        }

        [Command(Permission.EntitlementCharacter, "A collection of commands to manage character entitlements", "character")]
        public class EntitlementCommandCharacterCategory : CommandCategory
        {
            [Command(Permission.EntitlementCharacterAdd, "Create or update a character entitlement.", "add")]
            public void HandleEntitlementCommandCharacterAdd(ICommandContext context,
                [Parameter("Entitlement type to modify.", ParameterFlags.None, typeof(EnumParameterConverter<EntitlementType>))]
                EntitlementType entitlementType,
                [Parameter("Value to modify the entitlement.")]
                int value)
            {
                if (GameTableManager.Instance.Entitlement.GetEntry((ulong)entitlementType) == null)
                {
                    context.SendMessage($"{entitlementType} isn't a valid entitlement id!");
                    return;
                }

                context.GetTargetOrInvoker<Player>().Session.EntitlementManager.SetCharacterEntitlement(entitlementType, value);
            }

            [Command(Permission.EntitlementCharacterList, "List all entitlements for account.", "list")]
            public void HandleEntitlementCommandCharacterList(ICommandContext context)
            {
                Player player = context.GetTargetOrInvoker<Player>();
                context.SendMessage($"Entitlements for character {player.Session.Player.CharacterId}:");
                foreach (CharacterEntitlement entitlement in player.Session.EntitlementManager.GetCharacterEntitlements())
                    context.SendMessage($"Entitlement: {entitlement.Type}, Value: {entitlement.Amount}");
            }
        }
    }
}
