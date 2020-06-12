using System;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Account.Static;
using NexusForever.WorldServer.Game.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class PathHandler
    {
        [MessageHandler(GameMessageOpcode.ClientPathActivate)]
        public static void HandlePathActivate(WorldSession session, ClientPathActivate clientPathActivate)
        {
            uint activateCooldown = GameTableManager.Instance.GameFormula.GetEntry(2366).Dataint0;
            uint bypassCost       = GameTableManager.Instance.GameFormula.GetEntry(2366).Dataint01;
            bool needToUseTokens  = DateTime.UtcNow.Subtract(session.Player.PathActivatedTime).TotalSeconds < activateCooldown;

            GenericError CanActivatePath()
            {
                if (needToUseTokens && !clientPathActivate.UseTokens)
                    return GenericError.PathChangeOnCooldown;

                bool hasEnoughTokens = session.AccountCurrencyManager.CanAfford(AccountCurrencyType.ServiceToken, bypassCost); // TODO: Check user has enough tokens
                if (needToUseTokens && clientPathActivate.UseTokens && !hasEnoughTokens)
                    return GenericError.PathChangeInsufficientFunds;

                if (!session.Player.PathManager.IsPathUnlocked(clientPathActivate.Path))
                    return GenericError.PathChangeNotUnlocked;

                if (session.Player.PathManager.IsPathActive(clientPathActivate.Path))
                    return GenericError.PathChangeRequested;

                return GenericError.Ok;
            }

            GenericError result = CanActivatePath();
            if (result != GenericError.Ok)
            {
                session.Player.PathManager.SendServerPathActivateResult(result);
                return;
            }

            if (needToUseTokens && clientPathActivate.UseTokens)
                session.AccountCurrencyManager.CurrencySubtractAmount(AccountCurrencyType.ServiceToken, bypassCost);

            session.Player.PathManager.ActivatePath(clientPathActivate.Path);
        }

        [MessageHandler(GameMessageOpcode.ClientPathUnlock)]
        public static void HandlePathUnlock(WorldSession session, ClientPathUnlock clientPathUnlock)
        {
            uint unlockCost = GameTableManager.Instance.GameFormula.GetEntry(2365).Dataint0;

            GenericError CanUnlockPath()
            {
                bool hasEnoughTokens = session.AccountCurrencyManager.CanAfford(AccountCurrencyType.ServiceToken, unlockCost);
                if (!hasEnoughTokens)
                    return GenericError.PathInsufficientFunds;

                if (session.Player.PathManager.IsPathUnlocked(clientPathUnlock.Path))
                    return GenericError.PathAlreadyUnlocked;

                return GenericError.Ok;
            }

            GenericError result = CanUnlockPath();
            if (result != GenericError.Ok)
            {
                session.Player.PathManager.SendServerPathUnlockResult(result);
                return;
            }

            session.Player.PathManager.UnlockPath(clientPathUnlock.Path);
            session.AccountCurrencyManager.CurrencySubtractAmount(AccountCurrencyType.ServiceToken, unlockCost);
        }
    }
}
