using System.Linq;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Quest;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;
using NexusForever.Network.World.Message.Model;
using NLog;
using System;
using NexusForever.Game.Spell;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class EntityHandler
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        [MessageHandler(GameMessageOpcode.ClientEntityCommand)]
        public static void HandleEntityCommand(IWorldSession session, ClientEntityCommand entityCommand)
        {
            IWorldEntity mover = session.Player;
            if (mover == null)
                return;

            if (session.Player.ControlGuid != session.Player.Guid)
                mover = session.Player.GetVisible<IWorldEntity>(session.Player.ControlGuid);

            if (mover == null)
                return;

            foreach ((EntityCommand id, IEntityCommandModel command) in entityCommand.Commands)
            {
                switch (command)
                {
                    case SetPositionCommand setPosition:
                    {
                        // this is causing issues after moving to soon after mounting:
                        // session.Player.CancelSpellsOnMove();
                        mover.Relocate(setPosition.Position.Vector);
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
                ServerControlled = false,
                Commands = entityCommand.Commands
            });
        }

        [MessageHandler(GameMessageOpcode.ClientActivateUnit)]
        public static void HandleActivateUnit(IWorldSession session, ClientActivateUnit unit)
        {
            IWorldEntity entity = session.Player.GetVisible<IWorldEntity>(unit.UnitId);
            if (entity == null)
                throw new InvalidPacketValueException();

            // TODO: sanity check for range etc.

            entity.OnActivate(session.Player);
        }

        [MessageHandler(GameMessageOpcode.ClientEntityInteract)]
        public static void HandleClientEntityInteraction(WorldSession session, ClientEntityInteract entityInteraction)
        {
            IWorldEntity entity = session.Player.GetVisible<IWorldEntity>(entityInteraction.Guid);
            if (entity != null)
            {
                session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateEntity, entity.CreatureId, 1u);
                session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.TalkTo, entity.CreatureId, 1u);
                foreach (uint targetGroupId in AssetManager.Instance.GetTargetGroupsForCreatureId(entity.CreatureId) ?? Enumerable.Empty<uint>())
                    session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.TalkToTargetGroup, targetGroupId, 1u);
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
                    VendorHandler.HandleClientVendor(session, entityInteraction);
                    break;
                case 68: // "MailboxActivate"
                    var mailboxEntity = session.Player.Map.GetEntity<IMailbox>(entityInteraction.Guid);
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

        [MessageHandler(GameMessageOpcode.ClientEntityInteractChair)]
        public static void HandleClientEntityInteractEmote(IWorldSession session, ClientEntityInteractChair interactChair)
        {
            IWorldEntity chair = session.Player.GetVisible<IWorldEntity>(interactChair.ChairUnitId);
            if (chair == null)
                throw new InvalidPacketValueException();

            Creature2Entry creatureEntry = GameTableManager.Instance.Creature2.GetEntry(chair.CreatureId);
            if ((creatureEntry.ActivationFlags & 0x200000) == 0)
                throw new InvalidPacketValueException();

            session.Player.Sit(chair);
        }

        [MessageHandler(GameMessageOpcode.ClientActivateUnitCast)]
        public static void HandleActivateUnitCast(WorldSession session, ClientActivateUnitCast request)
        {
            IWorldEntity entity = session.Player.GetVisible<IWorldEntity>(request.ActivateUnitId);
            if (entity == null)
                throw new InvalidPacketValueException();

            entity.OnActivateCast(session.Player, request.ClientUniqueId);
        }

        /// <remarks>
        /// Possibly only used by Bindpoint entities
        /// </remarks>
        [MessageHandler(GameMessageOpcode.ClientActivateUnitInteraction)]
        public static void HandleActivateUnitDeferred(WorldSession session, ClientActivateUnitInteraction request)
        {
            IWorldEntity entity = session.Player.GetVisible<IWorldEntity>(request.ActivateUnitId);
            if (entity == null)
                throw new InvalidPacketValueException();

            entity.OnActivateCast(session.Player, request.ClientUniqueId);
        }

        [MessageHandler(GameMessageOpcode.ClientInteractionResult)]
        public static void HandleSpellDeferredResult(WorldSession session, ClientSpellInteractionResult result)
        {
            if (!(session.Player.HasSpell(x => x.CastingId == result.CastingId, out ISpell spell)))
                throw new ArgumentNullException($"Spell cast {result.CastingId} not found.");

            if (spell is not SpellClientSideInteraction spellCSI)
                throw new ArgumentNullException($"Spell missing a ClientSideInteraction.");

            switch (result.Result)
            {
                case 0:
                    spellCSI.FailClientInteraction();
                    break;
                case 1:
                    spellCSI.SucceedClientInteraction();
                    break;
                case 2:
                    spellCSI.CancelCast(CastResult.ClientSideInteractionFail);
                    break;
            }
        }
    }
}
