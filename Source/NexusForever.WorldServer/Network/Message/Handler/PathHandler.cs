using NexusForever.Shared.GameTable;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;
using System;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class PathHandler
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        [MessageHandler(GameMessageOpcode.ClientPathActivate)]
        public static void HandlePathActivate(WorldSession session, ClientPathActivate clientPathActivate)
        {
            Player player = session.Player;

            uint activateCooldown = GameTableManager.Instance.GameFormula.GetEntry(2366).Dataint0;
            uint bypassCost = GameTableManager.Instance.GameFormula.GetEntry(2366).Dataint01;

            bool needToUseTokens = DateTime.UtcNow.Subtract(player.PathActivatedTime).TotalSeconds < activateCooldown;
            GenericError errorCode = GenericError.Ok;
            bool hasEnoughTokens = session.AccountCurrencyManager.CanAfford(Game.Account.Static.AccountCurrencyType.ServiceToken, bypassCost); // TODO: Check user has enough tokens

            if (needToUseTokens && !clientPathActivate.UseTokens)
                errorCode = GenericError.PathChangeOnCooldown;

            if (needToUseTokens && clientPathActivate.UseTokens && !hasEnoughTokens)
                errorCode = GenericError.PathChangeInsufficientFunds;

            if (!player.PathManager.IsPathUnlocked(clientPathActivate.Path))
                errorCode = GenericError.PathChangeNotUnlocked;

            if (player.PathManager.IsPathActive(clientPathActivate.Path))
                errorCode = GenericError.PathChangeRequested;

            if (errorCode == GenericError.Ok)
            {
                if (needToUseTokens && clientPathActivate.UseTokens)
                    session.AccountCurrencyManager.CurrencySubtractAmount(Game.Account.Static.AccountCurrencyType.ServiceToken, bypassCost);

                player.PathManager.ActivatePath(clientPathActivate.Path);
            }
            else
                player.PathManager.SendServerPathActivateResult(errorCode);
            
        }

        [MessageHandler(GameMessageOpcode.ClientPathUnlock)]
        public static void HandlePathUnlock(WorldSession session, ClientPathUnlock clientPathUnlock)
        {
            Player player = session.Player;

            GenericError errorCode = GenericError.Ok;
            uint unlockCost = GameTableManager.Instance.GameFormula.GetEntry(2365).Dataint0;
            bool hasEnoughTokens = session.AccountCurrencyManager.CanAfford(Game.Account.Static.AccountCurrencyType.ServiceToken, unlockCost);

            if (!hasEnoughTokens)
                errorCode = GenericError.PathInsufficientFunds;

            if (player.PathManager.IsPathUnlocked(clientPathUnlock.Path))
                errorCode = GenericError.PathAlreadyUnlocked;

            if (errorCode == GenericError.Ok)
            {
                player.PathManager.UnlockPath(clientPathUnlock.Path);

                session.AccountCurrencyManager.CurrencySubtractAmount(Game.Account.Static.AccountCurrencyType.ServiceToken, unlockCost);
            }
            else
                player.PathManager.SendServerPathUnlockResult(errorCode);
        }
    }
}
