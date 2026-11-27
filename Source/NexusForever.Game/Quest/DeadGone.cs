using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.GameTable;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Quest
{
    internal static class DeadGone
    {
        internal const ushort QuestId = 6432;
        private const uint WorldId = 1421;
        private const uint FatalisFields = 1476;
        private const uint AftCompartment = 1539;
        private const uint Stoneform = 28875;
        private const uint CageDoor = 28887;
        private const uint TraceMarker = 22305;
        private const uint TraceObjective = 9883;

        internal static bool TryOpen(IPlayer player, ISimpleEntity actor)
        {
            if (actor.Map?.Entry.Id != WorldId)
                return false;

            bool available;
            if (Matches(actor, Stoneform, FatalisFields))
                available = MayDiscover(player);
            else if (Matches(actor, CageDoor, AftCompartment))
                available = player.QuestManager.GetQuestState(QuestId) == QuestState.Achieved;
            else
                return false;

            if (available && Near(player, actor))
                player.Session.EnqueueMessageEncrypted(new ServerDialogStart { DialogUnitId = actor.Guid });
            return true;
        }

        internal static bool CanAccept(IPlayer player) =>
            MayDiscover(player) && player.GetVisibleCreature<ISimpleEntity>(Stoneform)
                .Any(actor => Matches(actor, Stoneform, FatalisFields) && Near(player, actor));

        internal static bool CanTurnIn(IPlayer player) =>
            player.GetVisibleCreature<ISimpleEntity>(CageDoor)
                .Any(actor => Matches(actor, CageDoor, AftCompartment) && Near(player, actor));

        internal static void CheckTrace(IPlayer player, IQuest quest)
        {
            if (quest is not { State: QuestState.Accepted, PendingDelete: false } ||
                player is not { InWorld: true, IsAlive: true, IsLoading: false, Map.Entry.Id: WorldId, Zone.Id: AftCompartment } ||
                GameTableManager.Instance.WorldLocation2.GetEntry(TraceMarker) is not
                    { WorldId: WorldId, WorldZoneId: AftCompartment, Radius: > 0f } marker)
                return;

            var center = new Vector3(marker.Position0, marker.Position1, marker.Position2);
            if (Vector3.DistanceSquared(player.Position, center) <= marker.Radius * marker.Radius)
                quest.ObjectiveUpdate(TraceObjective, 1u);
        }

        private static bool MayDiscover(IPlayer player) =>
            GlobalQuestManager.Instance.GetQuestInfo(QuestId) is { } info &&
            player.Level >= info.Entry.PrerequisiteLevel &&
            player.QuestManager.GetQuestState(QuestId) is null or QuestState.Unknown or QuestState.Abandoned;

        private static bool Matches(ISimpleEntity actor, uint creature, uint zone) =>
            actor is { EntityId: > 0u, Map.Entry.Id: WorldId } && actor.CreatureId == creature && actor.Zone?.Id == zone;

        private static bool Near(IPlayer player, ISimpleEntity actor) =>
            player is { InWorld: true, IsAlive: true, IsLoading: false } && ReferenceEquals(player.Map, actor.Map) &&
            Vector3.DistanceSquared(player.Position, actor.Position) <= 25f;
    }
}
