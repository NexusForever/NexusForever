using System;
using NexusForever.Game.Static.Account;
using NexusForever.GameTable;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.WorldServer.Network.Message.Handler.Path
{
    public class ClientPathActivateHandler : IMessageHandler<IWorldSession, ClientPathActivate>
    {
        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;

        public ClientPathActivateHandler(
            IGameTableManager gameTableManager)
        {
            this.gameTableManager = gameTableManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientPathActivate clientPathActivate)
        {
            uint activateCooldown = gameTableManager.GameFormula.GetEntry(2366).Dataint0;
            uint bypassCost       = gameTableManager.GameFormula.GetEntry(2366).Dataint01;
            bool needToUseTokens  = DateTime.UtcNow.Subtract(session.Player.PathActivatedTime).TotalSeconds < activateCooldown;

            GenericError CanActivatePath()
            {
                if (needToUseTokens && !clientPathActivate.UseTokens)
                    return GenericError.PathChangeOnCooldown;

                bool hasEnoughTokens = session.Account.CurrencyManager.CanAfford(AccountCurrencyType.ServiceToken, bypassCost); // TODO: Check user has enough tokens
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
                session.Account.CurrencyManager.CurrencySubtractAmount(AccountCurrencyType.ServiceToken, bypassCost);

            session.Player.PathManager.ActivatePath(clientPathActivate.Path);
        }
    }
}
