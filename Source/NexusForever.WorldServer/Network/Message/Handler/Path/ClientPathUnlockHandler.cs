using NexusForever.Game.Static.Account;
using NexusForever.GameTable;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.WorldServer.Network.Message.Handler.Path
{
    public class ClientPathUnlockHandler : IMessageHandler<IWorldSession, ClientPathUnlock>
    {
        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;

        public ClientPathUnlockHandler(
            IGameTableManager gameTableManager)
        {
            this.gameTableManager = gameTableManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientPathUnlock clientPathUnlock)
        {
            uint unlockCost = gameTableManager.GameFormula.GetEntry(2365).Dataint0;

            GenericError CanUnlockPath()
            {
                bool hasEnoughTokens = session.Account.CurrencyManager.CanAfford(AccountCurrencyType.ServiceToken, unlockCost);
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
            session.Account.CurrencyManager.CurrencySubtractAmount(AccountCurrencyType.ServiceToken, unlockCost);
        }
    }
}
