using System.Numerics;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Quest
{
    public static class CryoAwakening
    {
        private const ushort Refuge = 3460;
        private const uint Noelia = 47431u;
        private const uint HoloBot = 47436u;

        private sealed record Room(ushort Quest, ushort World, ushort Zone, Vector3 RefugeGuide, Vector3 ShipGuide,
            Vector3 Arrival, Vector3 Arena1, Vector3 Arena2);

        private static readonly Room Exile = new(10045, 1537, 4813,
            new(-4147.65f, -449.5546f, -4191.1f), new(4605.66f, 5.60262f, 353.48492f),
            new(4605.66f, 5.60262f, 359.48492f), new(4556.861f, 9.63291f, 353.30493f),
            new(4654.369f, 9.63260f, 353.7249f));

        private static readonly Room Dominion = new(10108, 1634, 4844,
            new(-3568.3f, -436.93027f, -4314.8f), new(4088.3083f, 5.60208f, -145.34308f),
            new(4088.3083f, 5.60208f, -139.35909f), new(4039.6748f, 9.6324f, -145.44908f),
            new(4137.282f, 9.63391f, -146.49107f));

        private static Room RoomFor(IPlayer player) => player.Faction1 == player.Faction2 ? player.Faction1 switch
        {
            Faction.Exile => Exile,
            Faction.Dominion => Dominion,
            _ => null
        } : null;

        public static bool TryTravel(IPlayer player, IWorldEntity guide)
        {
            if (guide is not { CreatureId: Noelia, EntityId: > 0u } || guide.Map == null)
                return false;

            bool refugeeGuide = At(guide, Refuge, Exile.RefugeGuide) || At(guide, Refuge, Dominion.RefugeGuide);
            bool shipGuide = At(guide, Exile.World, Exile.ShipGuide) || At(guide, Dominion.World, Dominion.ShipGuide);
            if (!refugeeGuide && !shipGuide)
                return false;

            Room room = RoomFor(player);
            if (room == null || !CloseToGuide(player, guide))
                return true;

            bool completed = player.QuestManager.GetQuestState(room.Quest) == QuestState.Completed;
            if (refugeeGuide)
            {
                if (!completed && At(guide, Refuge, room.RefugeGuide))
                    Teleport(player, room.World, room.Arrival);
                return true;
            }

            if (!At(guide, room.World, room.ShipGuide))
                return true;
            if (!completed)
                return false;

            Teleport(player, Refuge, room.RefugeGuide + new Vector3(1.2f, .1f, 0f));
            return true;
        }

        public static bool AllowsContact(IPlayer player, ushort quest, bool remote)
        {
            if (quest != Exile.Quest && quest != Dominion.Quest)
                return true;

            return !remote && RoomFor(player) is { } room && room.Quest == quest &&
                player.GetVisibleCreature<IWorldEntity>(Noelia).Any(guide => At(guide, room.World, room.ShipGuide) &&
                    CloseToGuide(player, guide));
        }

        internal static bool IsTrainingBot(EntityModel model)
        {
            if (model is not { Creature: HoloBot, Type: EntityType.NonPlayer })
                return false;

            return InArena(Exile, model) || InArena(Dominion, model);
        }

        internal static bool CanFight(IPlayer player, IWorldEntity bot)
        {
            if (RoomFor(player) is not { } room || bot.Map?.Entry.Id != room.World || !ReferenceEquals(bot.Map, player.Map) ||
                player is not { InWorld: true, IsAlive: true, IsLoading: false } ||
                MathF.Abs(player.Position.Y - bot.Position.Y) > 3f)
                return false;

            return player.QuestManager.GetQuestState(room.Quest) == QuestState.Accepted;
        }

        private static bool InArena(Room room, EntityModel model) => model.World == room.World && model.Area == room.Zone &&
            (Near(new(model.X, model.Y, model.Z), room.Arena1, 8f) || Near(new(model.X, model.Y, model.Z), room.Arena2, 8f));

        private static bool At(IWorldEntity guide, ushort world, Vector3 position) => guide.Map?.Entry.Id == world &&
            Near(guide.LeashPosition, position, 3f);

        private static bool CloseToGuide(IPlayer player, IWorldEntity guide) =>
            player is { InWorld: true, IsAlive: true, IsLoading: false } && ReferenceEquals(player.Map, guide.Map) &&
            Near(player.Position, guide.Position, 6f);

        private static bool Near(Vector3 first, Vector3 second, float distance) =>
            Vector3.DistanceSquared(first, second) <= distance * distance;

        private static void Teleport(IPlayer player, ushort world, Vector3 destination)
        {
            if (!player.InCombat && player.CanTeleport() && GameTableManager.Instance.World.GetEntry(world) is WorldEntry entry &&
                !DisableManager.Instance.IsDisabled(DisableType.World, world))
                player.TeleportTo(entry, destination);
        }
    }
}
