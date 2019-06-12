using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Command;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;
using System;
using NexusForever.WorldServer.Game.Quest.Static;
using NexusForever.WorldServer.Game;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class EntityHandler
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        [MessageHandler(GameMessageOpcode.ClientEntityCommand)]
        public static void HandleEntityCommand(WorldSession session, ClientEntityCommand entityCommand)
        {
            WorldEntity mover = session.Player;
            if (session.Player.ControlGuid != session.Player.Guid)
                mover = session.Player.GetVisible<WorldEntity>(session.Player.ControlGuid);

            foreach ((EntityCommand id, IEntityCommandModel command) in entityCommand.Commands)
            {
                switch (command)
                {
                    case SetPositionCommand setPosition:
                    {
                        // this is causing issues after moving to soon after mounting:
                        // session.Player.CancelSpellsOnMove();

                        mover.Map.EnqueueRelocate(mover, setPosition.Position.Vector);
                        break;
                    }
                    case SetRotationCommand setRotation:
                        mover.Rotation = setRotation.Position.Vector;
                        break;
                }
            }

            mover.EnqueueToVisible(new ServerEntityCommand
            {
                Guid     = mover.Guid,
                Time     = entityCommand.Time,
                ServerControlled = true,
                Commands = entityCommand.Commands
            });
        }

        [MessageHandler(GameMessageOpcode.ClientActivateUnit)]
        public static void HandleActivateUnit(WorldSession session, ClientActivateUnit unit)
        {
            WorldEntity entity = session.Player.GetVisible<WorldEntity>(unit.UnitId);
            if (entity == null)
                throw new InvalidPacketValueException();

            // TODO: sanity check for range etc.

            entity.OnActivate(session.Player);
        }

        [MessageHandler(GameMessageOpcode.ClientActivateUnitCast)]
        public static void HandleActivateUnitCast(WorldSession session, ClientActivateUnitCast unit)
        {
            WorldEntity entity = session.Player.GetVisible<WorldEntity>(unit.ActivateUnitId);
            if (entity == null)
                throw new InvalidPacketValueException();

            // TODO: sanity check for range etc.

            session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateEntity, entity.CreatureId, 1u);
            foreach(uint targetGroupId in TargetGroupManager.GetTargetGroupsForCreatureId(entity.CreatureId))
                session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateTargetGroup, targetGroupId, 1u); // Updates the objective, but seems to disable all the other targets. TODO: Investigate
            entity.OnActivateCast(session.Player);
        }

        [MessageHandler(GameMessageOpcode.ClientEntityInteract)]
        public static void HandleClientEntityInteraction(WorldSession session, ClientEntityInteract entityInteraction)
        {
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
                    VendorHandler.HandleClientVendor(session, entityInteraction);
                    break;
                case 68: // "MailboxActivate"
                    var mailboxEntity = session.Player.Map.GetEntity<Mailbox>(entityInteraction.Guid);
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
                    log.Warn($"Received unhandled interaction event {entityInteraction.Event} from Entity {entityInteraction.Guid}");
                    break;
            }
        }
    }
}
