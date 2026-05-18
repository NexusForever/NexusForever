using System;
using NexusForever.Database;
using NexusForever.Database.Auth;
using NexusForever.Game.Abstract.Character;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Character;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.RBAC;
using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Ban, "A collection of commands to manage bans.", "ban")]
    public class BanCommandCategory : CommandCategory
    {
        [Command(Permission.BanAccount, "A collection of commands to manage account bans.", "account")]
        public class BanAccountCommandCategory : CommandCategory
        {
            [Command(Permission.BanAccountPlayer, "Ban an account of a player.", "player")]
            [CommandTarget(typeof(IPlayer))]
            public void HandleBanAccountPlayer(ICommandContext context,
                [Parameter("Reason for ban.")]
                string reason,
                [Parameter("Ban expiry time.")]
                DateTime? bannedTill)
            {
                // can't ban yourself...
                IPlayer player = context.GetTargetOrInvoker<IPlayer>();
                if (context.Invoker == player)
                    return;

                DatabaseManager.Instance.GetDatabase<AuthDatabase>().BanAccount(player.Account.Id, reason, bannedTill);

                player.Session.ForceDisconnect();

                context.SendMessage($"Account {player.Account.Id} for player {player.Name} was banned!");
            }

            [Command(Permission.BanAccountCharacter, "Ban an account of a character.", "character")]
            public void HandleBanAccountCharacter(ICommandContext context,
                [Parameter("Name of character that belongs to account to be banned.")]
                string name,
                [Parameter("Reason for ban.")]
                string reason,
                [Parameter("Ban expiry time.")]
                DateTime? bannedTill)
            {
                ICharacter character = CharacterManager.Instance.GetCharacter(name);
                if (character == null)
                {
                    context.SendError($"Character with the name {name} doesn't exist!");
                    return;
                }

                // can't ban yourself...
                if (context.Invoker is IPlayer player && player.Account.Id == character.AccountId)
                    return;

                DatabaseManager.Instance.GetDatabase<AuthDatabase>().BanAccount(character.AccountId, reason, bannedTill);

                IPlayer target = PlayerManager.Instance.GetPlayer(name);
                target?.Session.ForceDisconnect();

                context.SendMessage($"Account {character.AccountId} for character {character.Name} was banned!");
            }
        }
    }
}
