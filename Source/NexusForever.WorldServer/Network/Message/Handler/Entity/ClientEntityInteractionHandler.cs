using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
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
            IWorldEntity entity = session.Player.GetVisible<IWorldEntity>(entityInteraction.UnitId);
            if (entity != null)
            {
                session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateEntity, entity.CreatureId, 1u);
                session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.TalkTo, entity.CreatureId, 1u);
                session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.SucceedCSI, entity.CreatureId, 1u);
                foreach (uint targetGroupId in assetManager.GetTargetGroupsForCreatureId(entity.CreatureId) ?? Enumerable.Empty<uint>())
                {
                    session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.TalkToTargetGroup, targetGroupId, 1u);
                    if (entity is ISimpleEntity simple)
                        session.Player.QuestManager.ObjectiveChecklistBit(targetGroupId, simple.QuestChecklistIdx);
                }
            }

            switch (entityInteraction.Type)
            {
                case InteractionType.TalkTo: // Quest NPC
                {
                    session.EnqueueMessageEncrypted(new ServerDialogStart
                    {
                        DialogUnitId = entityInteraction.UnitId
                    });

                    // NPE housing hologram: skybox/music "upload" casts (client shows Uploading… until DialogEnd).
                    if (entity != null)
                        TryCastHousingTutorialSpell(session.Player, entity.CreatureId);
                    break;
                }
                case InteractionType.Vendor: // Handle Vendor
                    HandleVendor(session, entity);
                    break;
                case InteractionType.Mail: // "MailboxActivate"
                    var mailboxEntity = session.Player.Map.GetEntity<IMailboxEntity>(entityInteraction.UnitId);
                    break;
                case InteractionType.HousingGuildBroker: // "HousingGuildNeighborhoodBrokerOpen"
                case InteractionType.ConvertItem:
                case InteractionType.ConvertRep: // "ResourceConversionOpen"
                case InteractionType.Trainer: // "ToggleAbilitiesWindow"
                case InteractionType.TradeskillTrainer: // "InvokeTradeskillTrainerWindow"
                case InteractionType.Shuttle: // "InvokeShuttlePrompt"
                case InteractionType.FlightPathSettler:
                case InteractionType.FlightPathNew:
                case InteractionType.FlightPath: // "InvokeTaxiWindow"
                case InteractionType.Mannequin: // "MannequinWindowOpen"
                case InteractionType.Bank: // "ShowBank"
                case InteractionType.SharedRealmBank: // "ShowRealmBank"
                case InteractionType.Dye: // "ShowDye"
                case InteractionType.GuildRegistrar: // "GuildRegistrarOpen"
                case InteractionType.WarPartyRegistrar: // "WarPartyRegistrarOpen"
                case InteractionType.GuildBank: // "GuildBankerOpen"
                case InteractionType.WarPartyBank: // "WarPartyBankerOpen"
                case InteractionType.CommodityMarketplace: // "ToggleMarketplaceWindow"
                case InteractionType.ItemAuctionhouse: // "ToggleAuctionWindow"
                case InteractionType.EngravingStation: // "TradeskillEngravingStationOpen"
                case InteractionType.HousingMannequin: // "HousingMannequinOpen"
                case InteractionType.CityDirections: // "CityDirectionsList"
                case InteractionType.CREDDExchange: // "ToggleCREDDExchangeWindow"
                case InteractionType.CommunityRegistrar: // "CommunityRegistrarOpen"
                case InteractionType.ContractBoard: // "ContractBoardOpen"
                case InteractionType.Barber: // "BarberOpen"
                case InteractionType.MasterCraftsman: // "MasterCraftsmanOpen"
                default:
                    log.LogWarning($"Received unhandled interaction event {entityInteraction.Type} from Entity {entityInteraction.UnitId}");
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

        /// <summary>
        /// NPEU Part 4 housing specialists — skybox (73663) / music (73664) change spells.
        /// Landscape (73422) and Decor (73662) only need TalkToTargetGroup credit (count 2 each).
        /// </summary>
        private static void TryCastHousingTutorialSpell(IPlayer player, uint creatureId)
        {
            uint spellId = creatureId switch
            {
                73663 => 85631u, // NPEU - Part 4 - Housing - Skybox Change
                73664 => 85632u, // NPEU - Part 4 - Housing - Music Change
                _     => 0u
            };

            if (spellId == 0u)
                return;

            player.CastSpell(spellId);
        }
    }
}
