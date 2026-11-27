using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.Game.Quest
{
    public static class EverstarPassage
    {
        private const ushort QuestId = 6844;
        private const uint ArrivalObjective = 12559;
        private const ushort CelestionWorld = 51;
        private const uint ArrivalZone = 1558;
        private static readonly Vector3 portalPosition = new(650.2977f, -957.5671f, -3289.503f);
        private static readonly Vector3 landing = new(1000f, -928.25f, -2933f);

        /// <summary>
        /// Handle the quest portal at Queen Myala's platform.
        /// </summary>
        public static bool TryBegin(IPlayer player, IWorldEntity portal)
        {
            if (portal is not ISimpleEntity
                {
                    EntityId: > 0u,
                    CreatureId: 32269u,
                    Map.Entry.Id: 990u,
                    Zone.Id: 1420u,
                    CreatureEntry.Spell4IdActivate00: 33983u
                } || !(Vector3.DistanceSquared(portal.Position, portalPosition) <= .25f))
                return false;

            if (CanTravel(player, portal))
                QuestCsiInteraction.TryBegin(player, portal, Complete);
            return true;
        }

        private static bool CanTravel(IPlayer player, IWorldEntity portal)
        {
            return player is { InWorld: true, IsAlive: true, IsLoading: false, InCombat: false, Faction1: Faction.Exile }
                && ReferenceEquals(player.Map, portal.Map) && player.CanTeleport()
                && player.QuestManager.GetActiveQuests().Any(quest =>
                    quest is { Id: QuestId, State: QuestState.Accepted, PendingDelete: false } &&
                    quest.Any(goal =>
                        goal.ObjectiveInfo is { Id: ArrivalObjective, Type: QuestObjectiveType.EnterZone, Entry.Data: ArrivalZone }
                        && !goal.IsComplete()));
        }

        private static void Complete(IPlayer player, IWorldEntity portal)
        {
            if (CanTravel(player, portal))
                player.TeleportTo(CelestionWorld, landing.X, landing.Y, landing.Z);
        }
    }
}
