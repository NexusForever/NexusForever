using System.Collections.Immutable;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable.Model;
using NexusForever.Network.Session;
using NexusForever.Network.World.Message.Model.Story;

namespace NexusForever.Game.Quest
{
    public class CommunicatorMessage : ICommunicatorMessage
    {
        public uint Id => Entry.Id;
        public ushort QuestId => (ushort)Entry.QuestIdDelivered;
        public CommunicatorMessagesEntry Entry { get; }
        public CommunicatorKind Kind { get; }
        public bool ServerPushed => Kind is CommunicatorKind.PushedScheduled;
        // Unconditioned server-pushed rows wait on a script: nothing sends them, and the client drops them, so no player
        // can have seen one.
        public bool Reachable => Kind is CommunicatorKind.PushedScheduled or CommunicatorKind.ClientScheduled
            or CommunicatorKind.ClientUnscheduled;
        public bool Delivers => Entry.QuestIdDelivered != 0u;
        public ImmutableArray<CommunicatorCondition> Conditions { get; }
        public ImmutableArray<CommunicatorTrigger> Triggers { get; }

        /// <summary>
        /// Create a new <see cref="ICommunicatorMessage"/> classified by <see cref="CommunicatorIndex"/>.
        /// </summary>
        public CommunicatorMessage(CommunicatorMessagesEntry entry, CommunicatorKind kind,
            ImmutableArray<CommunicatorCondition> conditions, ImmutableArray<CommunicatorTrigger> triggers)
        {
            Entry      = entry;
            Kind       = kind;
            Conditions = conditions;
            Triggers   = triggers;
        }

        /// <summary>
        /// Evaluate the row's conditions for <paramref name="player"/> in the client's order (0x14043d3d0), skipping <paramref name="skip"/>.
        /// </summary>
        public CommunicatorResult Evaluate(ICommunicatorPlayerView player, CommunicatorGates skip = CommunicatorGates.None)
        {
            if ((skip & CommunicatorGates.World) == 0 && Entry.WorldId != 0u && Entry.WorldId != player.WorldId)
                return CommunicatorResult.World;
            if ((skip & CommunicatorGates.Zone) == 0 && Entry.WorldZoneId != 0u && !player.IsInZone(Entry.WorldZoneId))
                return CommunicatorResult.Zone;
            if (Entry.ClassId != 0u && Entry.ClassId != (uint)player.Class)
                return CommunicatorResult.Class;
            if (Entry.RaceId != 0u && Entry.RaceId != (uint)player.Race)
                return CommunicatorResult.Race;
            if (Entry.FactionId != 0u && Entry.FactionId != (uint)player.Faction)
                return CommunicatorResult.Faction;

            if (Entry.FactionIdReputation != 0u)
            {
                // The client's inequality here is unverified.
                float amount = player.GetReputation((Faction)Entry.FactionIdReputation);
                if (Entry.ReputationMin != 0u && amount < Entry.ReputationMin
                    || Entry.ReputationMax != 0u && amount > Entry.ReputationMax)
                    return CommunicatorResult.Reputation;
            }

            if (Entry.MaxLevel != 0u && player.Level > Entry.MaxLevel)
                return CommunicatorResult.MaxLevel;
            if (Entry.MinLevel != 0u && player.Level < Entry.MinLevel)
                return CommunicatorResult.MinLevel;

            foreach (CommunicatorCondition condition in Conditions)
                if (!CommunicatorConditions.Meets(condition.TableState, condition.Quest, player.GetQuest(condition.QuestId)))
                    return CommunicatorResult.QuestCondition;

            if (Entry.PrerequisiteId != 0u && !player.MeetsPrerequisite(Entry.PrerequisiteId))
                return CommunicatorResult.Prerequisite;

            if ((skip & CommunicatorGates.Offer) == 0 && Delivers && player.CheckOffer(QuestId) != CommunicatorOfferBlock.None)
                return CommunicatorResult.Offer;

            return CommunicatorResult.Eligible;
        }

        /// <summary>
        /// Send communicator message to <see cref="IGameSession"/>.
        /// </summary>
        public void Send(IGameSession session, bool checkConditions = true)
        {
            session.EnqueueMessageEncrypted(new ServerCommunicatorMessage
            {
                CommunicatorMessagesId = (ushort)Entry.Id,
                CheckConditions        = checkConditions
            });
        }
    }
}
