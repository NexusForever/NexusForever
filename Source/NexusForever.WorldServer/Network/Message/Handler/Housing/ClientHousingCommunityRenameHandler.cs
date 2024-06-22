using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Text.Filter;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.TextFilter;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingCommunityRenameHandler : IMessageHandler<IWorldSession, ClientHousingCommunityRename>
    {
        #region Dependency Injection

        private readonly ITextFilterManager textFilterManager;
        private readonly IGameTableManager gameTableManager;
        private readonly IRealmContext realmContext;

        public ClientHousingCommunityRenameHandler(
            ITextFilterManager textFilterManager,
            IGameTableManager gameTableManager,
            IRealmContext realmContext)
        {
            this.textFilterManager = textFilterManager;
            this.gameTableManager  = gameTableManager;
            this.realmContext      = realmContext;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientHousingCommunityRename housingCommunityRename)
        {
            if (session.Player.Map is not IResidenceMapInstance)
                throw new InvalidPacketValueException();

            // ignore the value in the packet
            ICommunity community = session.Player.GuildManager.GetGuild<ICommunity>(GuildType.Community);
            if (community == null)
                throw new InvalidPacketValueException();

            HousingResult GetResult()
            {
                // client checks if the player has a rank of 0, this is the same
                if (community.LeaderId != session.Player.CharacterId)
                    return HousingResult.InvalidPermissions;

                if (!textFilterManager.IsTextValid(housingCommunityRename.Name)
                    || !textFilterManager.IsTextValid(housingCommunityRename.Name, UserText.HousingResidenceName))
                    return HousingResult.InvalidResidenceName;

                GameFormulaEntry entry = gameTableManager.GameFormula.GetEntry(2395);
                if (entry == null)
                    return HousingResult.Failed;

                bool canAfford;
                if (housingCommunityRename.AlternativeCurrency)
                    canAfford = session.Player.CurrencyManager.CanAfford(CurrencyType.Renown, entry.Dataint01);
                else
                    canAfford = session.Player.CurrencyManager.CanAfford(CurrencyType.Credits, entry.Dataint0);

                if (!canAfford)
                    return HousingResult.InsufficientFunds;

                return HousingResult.Success;
            }

            HousingResult result = GetResult();
            if (result == HousingResult.Success)
            {
                // fun fact: 2395 is the final game formula entry
                GameFormulaEntry entry = gameTableManager.GameFormula.GetEntry(2395);
                if (housingCommunityRename.AlternativeCurrency)
                    session.Player.CurrencyManager.CurrencySubtractAmount(CurrencyType.Renown, entry.Dataint01);
                else
                    session.Player.CurrencyManager.CurrencySubtractAmount(CurrencyType.Credits, entry.Dataint0);

                community.RenameGuild(housingCommunityRename.Name);
                community.Residence.Map?.RenameResidence(community.Residence, housingCommunityRename.Name);
            }

            session.EnqueueMessageEncrypted(new ServerHousingCommunityRename
            {
                Result      = HousingResult.Success,
                TargetGuild = new TargetGuild
                {
                    RealmId = realmContext.RealmId,
                    GuildId = community.Id
                }
            });
        }
    }
}
