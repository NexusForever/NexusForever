using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Quest
{
    internal static class ArcanusSilo
    {
        private const ushort Exile = 6528;
        private const ushort Dominion = 6529;
        private const ushort World = 1421;
        private const ushort Ravine = 1424;
        private const ushort Silo = 1430;
        private const uint Notes = 34506;
        private static readonly Vector3 EntryGroundOffset = new(0, -4.8f, 0);
        private static readonly Vector3 ExitOffset = new(0, 0, 7);

        internal static bool Handles(ushort quest) => quest is Exile or Dominion;

        internal static bool TryOpen(IPlayer player, ISimpleEntity actor)
        {
            if (!IsNotes(actor))
                return false;

            ushort quest = For(player.Faction1);
            if (Near(player, actor) && CanStart(player, quest))
                player.Session.EnqueueMessageEncrypted(new ServerDialogStart { DialogUnitId = actor.Guid });
            return true;
        }

        internal static bool CanAccept(IPlayer player, ushort quest) =>
            For(player.Faction1) == quest && CanStart(player, quest) &&
            player.GetVisibleCreature<ISimpleEntity>(Notes).Any(actor => IsNotes(actor) && Near(player, actor));

        internal static void Update(IPlayer player, IQuest quest)
        {
            if (player is not { InWorld: true, IsAlive: true, IsLoading: false, Map.Entry.Id: World,
                Zone.Id: Ravine or Silo } || Entry(21036, Ravine) is not { } marker ||
                Entry(43195, Silo) is not { } arrival)
                return;

            Vector3 entrance = marker + EntryGroundOffset;
            bool active = quest is { State: QuestState.Accepted, PendingDelete: false };
            if (player.Zone.Id == Silo && active && Vector3.DistanceSquared(player.Position, arrival) <= 25f)
                quest.ObjectiveUpdate(quest.Id == Exile ? 13122u : 13123u, 1u);

            if (player.InCombat || !player.CanTeleport())
                return;

            Vector3 exit = arrival + ExitOffset;
            if (player.Zone.Id == Silo && Vector3.DistanceSquared(player.Position, exit) <= 9f)
                Transfer(player, entrance - ExitOffset);
            else if (player.Zone.Id == Ravine && active && Vector3.DistanceSquared(player.Position, entrance) <= 9f)
                Transfer(player, arrival);
        }

        internal static ushort For(Faction faction) => faction switch
        {
            Faction.Exile => Exile,
            Faction.Dominion => Dominion,
            _ => 0
        };

        private static Vector3? Entry(uint id, ushort zone) =>
            GameTableManager.Instance.WorldLocation2.GetEntry(id) is { WorldId: World, WorldZoneId: var area } marker && area == zone
                ? new Vector3(marker.Position0, marker.Position1, marker.Position2) : null;

        private static void Transfer(IPlayer player, Vector3 position) => player.TeleportTo(World, position.X, position.Y, position.Z);

        private static bool CanStart(IPlayer player, ushort quest) =>
            quest != 0 && GlobalQuestManager.Instance.GetQuestInfo(quest) is { } info && player.Level >= info.Entry.PrerequisiteLevel &&
            player.QuestManager.GetQuestState(quest) is null or QuestState.Unknown or QuestState.Abandoned;

        private static bool IsNotes(ISimpleEntity actor) =>
            actor is { EntityId: > 0u, CreatureId: Notes, Map.Entry.Id: World, Zone.Id: Ravine };

        private static bool Near(IPlayer player, ISimpleEntity actor) =>
            player is { InWorld: true, IsAlive: true, IsLoading: false } && ReferenceEquals(player.Map, actor.Map) &&
            Vector3.DistanceSquared(player.Position, actor.Position) <= 25f;
    }
}
