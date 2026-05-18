using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Game.Abstract.Customisation;
using NexusForever.Game.Static.Account;
using NexusForever.Game.Static.Entity;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity.Player
{
    public class ClientAppearanceChangeHandler : IMessageHandler<IWorldSession, ClientCharacterAppearanceChange>
    {
        #region Dependency Injection

        private readonly ICustomisationManager customisationManager;

        public ClientAppearanceChangeHandler(
            ICustomisationManager customisationManager)
        {
            this.customisationManager = customisationManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientCharacterAppearanceChange appearanceChange)
        {
            List<(uint Label, uint Value)> customisations = appearanceChange.Labels
                .Zip(appearanceChange.Values, ValueTuple.Create)
                .ToList();

            if (!customisationManager.Validate(appearanceChange.Race, appearanceChange.Sex, session.Player.Faction1, customisations))
                throw new InvalidPacketValueException();

            if (appearanceChange.UseServiceTokens)
            {
                uint cost = customisationManager.CalculateCostTokens(session.Player, appearanceChange.Race, appearanceChange.Sex);
                if (!session.Player.Account.CurrencyManager.CanAfford(AccountCurrencyType.ServiceToken, cost))
                    return;

                session.Player.Account.CurrencyManager.CurrencySubtractAmount(AccountCurrencyType.ServiceToken, cost);
            }
            else
            {
                uint cost = customisationManager.CalculateCostCredits(session.Player, appearanceChange.Race, appearanceChange.Sex, customisations, appearanceChange.Bones);
                if (!session.Player.CurrencyManager.CanAfford(CurrencyType.Credits, cost))
                    return;

                session.Player.CurrencyManager.CurrencySubtractAmount(CurrencyType.Credits, cost);
            }

            session.Player.AppearanceManager.Update(appearanceChange.Race, appearanceChange.Sex, customisations, appearanceChange.Bones);
        }
    }
}
