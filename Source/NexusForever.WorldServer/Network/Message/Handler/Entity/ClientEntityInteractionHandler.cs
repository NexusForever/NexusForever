using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Event;
using NexusForever.Game.Static.Quest;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity
{
    public class ClientEntityInteractionHandler : IMessageHandler<IWorldSession, ClientEntityInteract>
    {
        #region Dependency Injection

        private readonly ILogger<ClientEntityInteractionHandler> log;

        private readonly IAssetManager assetManager;

        public ClientEntityInteractionHandler(
            ILogger<ClientEntityInteractionHandler> log,
            IAssetManager assetManager)
        {
            this.log          = log;
            this.assetManager = assetManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientEntityInteract entityInteraction)
        {
            IWorldEntity entity = session.Player.GetVisible<IWorldEntity>(entityInteraction.Guid);
            if (entity != null)
            {
                session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateEntity, entity.CreatureId, 1u);
                session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.TalkTo, entity.CreatureId, 1u);
                foreach (uint targetGroupId in assetManager.GetTargetGroupsForCreatureId(entity.CreatureId) ?? Enumerable.Empty<uint>())
                    session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.TalkToTargetGroup, targetGroupId, 1u);

                entity.Map.PublicEventManager.UpdateObjective(session.Player, PublicEventObjectiveType.TalkTo, entity.CreatureId, 1);
            }

            switch (entityInteraction.Event)
            {
                case 37: // Quest NPC
                {
                    session.EnqueueMessageEncrypted(new Server0357
                    {
                        UnitId = entityInteraction.Guid
                    });
                    break;
                }
                case 49: // Handle Vendor
                    HandleVendor(session, entity);
                    break;
                case 68: // "MailboxActivate"
                    var mailboxEntity = session.Player.Map.GetEntity<IMailboxEntity>(entityInteraction.Guid);
                    break;
                case 8: // "HousingGuildNeighborhoodBrokerOpen"
                case 40:
                case 41: // "ResourceConversionOpen"
                case 42: // "ToggleAbilitiesWindow"
                case 43: // "InvokeTradeskillTrainerWindow"
                case 45: // "InvokeShuttlePrompt"
                case 46:
                case 47:
                case 48: // "InvokeTaxiWindow"
                case 65: // "MannequinWindowOpen"
                case 66: // "ShowBank"
                case 67: // "ShowRealmBank"
                case 69: // "ShowDye"
                case 70: // "GuildRegistrarOpen"
                case 71: // "WarPartyRegistrarOpen"
                case 72: // "GuildBankerOpen"
                case 73: // "WarPartyBankerOpen"
                case 75: // "ToggleMarketplaceWindow"
                case 76: // "ToggleAuctionWindow"
                case 79: // "TradeskillEngravingStationOpen"
                case 80: // "HousingMannequinOpen"
                case 81: // "CityDirectionsList"
                case 82: // "ToggleCREDDExchangeWindow"
                case 84: // "CommunityRegistrarOpen"
                case 85: // "ContractBoardOpen"
                case 86: // "BarberOpen"
                case 87: // "MasterCraftsmanOpen"
                default:
                    log.LogWarning($"Received unhandled interaction event {entityInteraction.Event} from Entity {entityInteraction.Guid}");
                    break;
            }
        }

        private void HandleVendor(IWorldSession session, IWorldEntity worldEntity)
        {
            if (worldEntity is not INonPlayerEntity vendorEntity)
                throw new InvalidOperationException();

            if (vendorEntity.VendorInfo == null)
                throw new InvalidOperationException();

            session.Player.SelectedVendorInfo = vendorEntity.VendorInfo;

            ServerVendorItemsUpdated vendorItemsUpdated = vendorEntity.VendorInfo.Build();
            vendorItemsUpdated.Guid = vendorEntity.Guid;
            session.EnqueueMessageEncrypted(vendorItemsUpdated);
        }
    }
}
