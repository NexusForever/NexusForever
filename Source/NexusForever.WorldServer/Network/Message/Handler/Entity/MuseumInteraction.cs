using System.Linq;
using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity
{
    internal static class MuseumInteraction
    {
        private const ushort Illium = 22;
        private const ushort Thayd = 51;
        private const uint ImperialMuseumEntrance = 62464;
        private const uint ImperialMuseumExit = 62465;
        private const uint ImperialCurator = 53616;
        private const uint BlueHorizonCurator = 53439;

        private sealed record Exhibit(ushort Quest, uint Relic, ushort RelicWorld, uint Display, bool BlueHorizon = false)
        {
            internal ushort MuseumWorld => BlueHorizon ? Thayd : Illium;
            internal uint Curator => BlueHorizon ? BlueHorizonCurator : ImperialCurator;
            internal Faction Faction => BlueHorizon ? Faction.Exile : Faction.Dominion;
        }

        private static readonly Exhibit[] exhibits =
        [
            new(9092, 53366, 22, 53610),
            new(9129, 53681, 22, 53703),
            new(9138, 53797, 1061, 53803),
            new(9139, 53812, 51, 53813),
            new(9141, 53842, 1061, 53836),
            new(9143, 53840, 22, 53834),
            new(9144, 53833, 1421, 53837),
            new(9145, 53849, 1421, 53877, true),
            new(9146, 53855, 22, 53856),
            new(9147, 53851, 1061, 53848, true)
        ];

        internal static bool TryActivate(IWorldSession session, IWorldEntity actor)
        {
            if (actor?.Map?.Entry is not { } world)
                return false;

            IPlayer player = session.Player;
            if (world.Id == Illium && actor.CreatureId is ImperialMuseumEntrance or ImperialMuseumExit)
            {
                if (player.Faction1 == Faction.Dominion && !player.InCombat && !player.PlatformGuid.HasValue &&
                    player.CanTeleport() && IsNear(player, actor))
                {
                    if (actor.CreatureId == ImperialMuseumEntrance)
                        player.TeleportTo(Illium, -3381.194f, -1031.0392f, -1718.9211f);
                    else
                        player.TeleportTo(Illium, -3482.605f, -883.4597f, -840.8839f);
                }
                return true;
            }

            Exhibit exhibit = exhibits.FirstOrDefault(entry => entry.Relic == actor.CreatureId && entry.RelicWorld == world.Id);
            if (exhibit != null)
            {
                if (player.Faction1 == exhibit.Faction && IsNear(player, actor))
                    session.EnqueueMessageEncrypted(new ServerDialogStart { DialogUnitId = actor.Guid });
                return true;
            }

            exhibit = exhibits.FirstOrDefault(entry => entry.Display == actor.CreatureId && entry.MuseumWorld == world.Id);
            if (exhibit == null)
                return false;

            // The client defines SucceedCSI, but these displays use an instant activation with no CSI descriptor.
            if (actor.CreatureEntry is { Spell4IdActivate00: 1817u } && player.Faction1 == exhibit.Faction &&
                IsNear(player, actor) && player.QuestManager.GetQuestState(exhibit.Quest) == QuestState.Accepted)
                player.QuestManager.ObjectiveUpdate(QuestObjectiveType.SucceedCSI, exhibit.Display, 1u);
            return true;
        }

        internal static bool CanAccept(IPlayer player, ushort questId, bool fromItem)
        {
            Exhibit exhibit = exhibits.FirstOrDefault(entry => entry.Quest == questId);
            return exhibit == null || !fromItem && player.Faction1 == exhibit.Faction &&
                IsNextTo(player, exhibit.Relic, exhibit.RelicWorld);
        }

        internal static bool CanReport(IPlayer player, ushort questId, bool communicator)
        {
            Exhibit exhibit = exhibits.FirstOrDefault(entry => entry.Quest == questId);
            return exhibit == null || !communicator && player.Faction1 == exhibit.Faction &&
                IsNextTo(player, exhibit.Curator, exhibit.MuseumWorld);
        }

        private static bool IsNextTo(IPlayer player, uint creature, ushort world) =>
            player.Map?.Entry.Id == world && player.GetVisibleCreature<IWorldEntity>(creature)
                .Any(actor => IsNear(player, actor));

        private static bool IsNear(IPlayer player, IWorldEntity actor) =>
            actor.EntityId != 0u && actor.Guid != 0u && player.InWorld && player.IsAlive && !player.IsLoading &&
            ReferenceEquals(player.Map, actor.Map) && Vector3.DistanceSquared(player.Position, actor.Position) <= 25f;
    }
}
