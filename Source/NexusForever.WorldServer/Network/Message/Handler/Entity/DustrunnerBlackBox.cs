using System.Linq;
using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Prerequisite;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity
{
    internal static class DustrunnerBlackBox
    {
        private const ushort World = 1421;
        private const uint BlackBox = 29658;

        private sealed record Route(ushort Quest, Faction Faction, uint Condition, uint Receiver);
        private static readonly Route exile = new(6185, Faction.Exile, 17569, 27647);
        private static readonly Route dominion = new(6186, Faction.Dominion, 17570, 29360);

        internal static bool TryOpen(IWorldSession session, IWorldEntity actor)
        {
            if (actor is not ISimpleEntity { CreatureId: BlackBox, Map.Entry.Id: World })
                return false;

            IPlayer player = session.Player;
            Route route = player.Faction1 switch
            {
                Faction.Exile => exile,
                Faction.Dominion => dominion,
                _ => null
            };
            if (route != null && CanStart(player, route) && IsNear(player, actor))
                session.EnqueueMessageEncrypted(new ServerDialogStart { DialogUnitId = actor.Guid });
            return true;
        }

        internal static bool CanAccept(IPlayer player, ushort quest, bool fromItem)
        {
            Route route = GetRoute(quest);
            return route == null || !fromItem && CanStart(player, route) &&
                player.GetVisibleCreature<ISimpleEntity>(BlackBox).Any(actor => IsNear(player, actor));
        }

        internal static bool CanComplete(IPlayer player, ushort quest, bool communicator)
        {
            Route route = GetRoute(quest);
            return route == null || !communicator && player.Faction1 == route.Faction &&
                player.GetVisibleCreature<INonPlayerEntity>(route.Receiver)
                    .Any(actor => actor.IsAlive && IsNear(player, actor));
        }

        private static Route GetRoute(ushort quest) => quest switch
        {
            6185 => exile,
            6186 => dominion,
            _ => null
        };

        private static bool CanStart(IPlayer player, Route route) =>
            player.Faction1 == route.Faction &&
            player.QuestManager.GetQuestState(route.Quest) is null or QuestState.Unknown or QuestState.Mentioned or QuestState.Abandoned &&
            PrerequisiteManager.Instance.Meets(player, route.Condition);

        private static bool IsNear(IPlayer player, IWorldEntity actor) =>
            actor is { EntityId: > 0u, Map.Entry.Id: World } &&
            player is { InWorld: true, IsLoading: false, IsAlive: true } &&
            ReferenceEquals(player.Map, actor.Map) &&
            Vector3.DistanceSquared(player.Position, actor.Position) <= 25f;
    }
}
