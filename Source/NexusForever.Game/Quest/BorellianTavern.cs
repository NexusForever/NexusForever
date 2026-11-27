using System.Numerics;
using System.Runtime.CompilerServices;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Entity;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;

namespace NexusForever.Game.Quest
{
    public static class BorellianTavern
    {
        internal const ushort World = 1068;

        private sealed record Site(uint Creature, ushort Zone, Vector3 Position);
        private sealed record Mug(Site Site, uint Spell, uint Csi, uint[] Effects);
        private sealed record Round(uint Goal, uint Group, int Mugs);
        private sealed record Bar(ushort Quest, Faction Faction, Site Patron, Site Partner, uint Talk, uint FirstWait,
            uint LastWait, Mug[] Mugs, Round[] Rounds);

        private static readonly Bar exile = new(6251, Faction.Exile,
            new(29813,1543,new(-11757.218f,66.22217f,-4075.468f)),
            new(29356,1543,new(-11775.663f,66.64973f,-4037.7524f)),9940,9942,9946,
            [
                new(new(29171,1543,new(-11773.478f,67.225044f,-4039.4688f)),33716,299,[65819,65820,65821]),
                new(new(29315,1543,new(-11774.678f,67.225044f,-4039.4688f)),33742,300,[0,65838,65839]),
                new(new(29316,1543,new(-11772.278f,67.225044f,-4039.4688f)),33743,301,[0,0,65842])
            ],[new(9943,3259,1),new(9944,3260,2),new(9945,3261,3)]);
        private static readonly Bar dominion = new(6535, Faction.Dominion,
            new(29812,1542,new(-11961.307f,57.882256f,-4002.0625f)),
            new(29743,1542,new(-11980.569f,67.92763f,-4007.5098f)),10134,10135,10139,
            [
                new(new(29744,1542,new(-11979.402f,69.38753f,-4009.37f)),33966,299,[66567,66568,66569]),
                new(new(29745,1542,new(-11980.602f,69.38753f,-4009.37f)),33967,300,[0,66570,66571]),
                new(new(29746,1542,new(-11978.202f,69.38753f,-4009.37f)),33968,301,[0,0,66572])
            ],[new(10136,3345,1),new(10137,3346,2),new(10138,3347,3)]);

        private static readonly ConditionalWeakTable<IPlayer, PendingWait> waits = new();

        private sealed class PendingWait
        {
            internal uint Goal;
            internal double Seconds;
        }

        private static Bar For(IPlayer player) => player.Faction1 == player.Faction2 ? player.Faction1 switch
        {
            Faction.Exile => exile,
            Faction.Dominion => dominion,
            _ => null
        } : null;

        private static bool Valid(IPlayer player) => player is { InWorld: true, IsAlive: true, IsLoading: false, Map.Entry.Id: World };

        private static bool Is(IGridEntity entity, Site site) => entity is IWorldEntity { EntityId: > 0u, Map.Entry.Id: World } actor &&
            actor.CreatureId == site.Creature && actor.Zone?.Id == site.Zone && Vector3.DistanceSquared(site.Position,actor.LeashPosition) <= 2.25f;

        private static IWorldEntity Near(IPlayer player, Site site) => player.GetVisibleCreature<IWorldEntity>(site.Creature)
            .FirstOrDefault(actor => Is(actor,site) && QuestCsiInteraction.CanInteract(player,actor));

        private static IQuest Active(IPlayer player, ushort id) => player.QuestManager.GetActiveQuests().FirstOrDefault(quest =>
            quest.Id == id && quest is { State: QuestState.Accepted, PendingDelete: false });

        private static QuestObjective Goal(IQuest quest,uint id,QuestObjectiveType type,uint data) =>
            quest?.OfType<QuestObjective>().FirstOrDefault(goal => goal.ObjectiveInfo.Id == id &&
                goal.ObjectiveInfo.Type == type && goal.ObjectiveInfo.Entry.Data == data && !goal.IsComplete() && ((Quest)quest).CanUpdateObjective(goal));

        private static void Advance(IQuest quest, uint id, QuestObjectiveType type, uint data)
        {
            if (Goal(quest,id,type,data) != null)
                quest.ObjectiveUpdate(id,1u);
        }

        internal static bool HandlesQuest(uint quest) => quest is 6251u or 6535u;

        internal static bool OwnsObjective(ushort quest, uint goal) =>
            (quest, goal) is (6251, 9940u or >= 9942u and <= 9946u) or (6535, >= 10134u and <= 10139u);

        internal static bool AllowsContact(IPlayer player, uint quest) =>
            !HandlesQuest(quest) || Valid(player) && For(player) is { } bar && quest == bar.Quest &&
                Near(player, bar.Patron) != null;

        internal static bool Tracks(IGridEntity actor) =>
            Is(actor, exile.Patron) || Is(actor, dominion.Patron) || Is(actor, exile.Partner) ||
            Is(actor, dominion.Partner) || IsMug(actor);

        internal static bool CanSee(IPlayer player, IGridEntity actor) =>
            !Tracks(actor) || For(player) is { } bar &&
                (Is(actor, bar.Patron) || Active(player, bar.Quest) != null &&
                    (Is(actor, bar.Partner) || bar.Mugs.Any(mug => Is(actor, mug.Site))));

        public static void Talk(IPlayer player, IWorldEntity actor)
        {
            if (Valid(player) && For(player) is { } bar && Is(actor, bar.Partner) &&
                QuestCsiInteraction.CanInteract(player, actor))
                Advance(Active(player, bar.Quest), bar.Talk, QuestObjectiveType.TalkTo, actor.CreatureId);
        }

        internal static void Update(IPlayer player, double elapsed)
        {
            if (Valid(player) && For(player) is { } bar && Active(player, bar.Quest) is { } quest)
                UpdateWait(player, bar, quest, elapsed);
            else
                waits.Remove(player);
        }

        private static void UpdateWait(IPlayer player,Bar bar,IQuest quest,double elapsed)
        {
            QuestObjective objective = Goal(quest,bar.FirstWait,QuestObjectiveType.ActivateEntity,0u) ??
                Goal(quest,bar.LastWait,QuestObjectiveType.ActivateEntity,0u);
            if (objective == null || Near(player,bar.Partner) == null)
            {
                waits.Remove(player);
                return;
            }
            PendingWait wait = waits.GetOrCreateValue(player);
            uint goal = objective.ObjectiveInfo.Id;
            if (wait.Goal != goal)
            {
                wait.Goal = goal;
                wait.Seconds = 0d;
            }
            if ((wait.Seconds += elapsed) < 3d)
                return;

            quest.ObjectiveUpdate(goal,1u);
            waits.Remove(player);
        }

        internal static bool IsMug(IGridEntity actor) =>
            exile.Mugs.Any(mug => Is(actor,mug.Site)) || dominion.Mugs.Any(mug => Is(actor,mug.Site));

        internal static (IQuest Quest, QuestObjective Goal, uint Spell, uint Csi, uint Effect, uint Bit) PrepareCsi(IPlayer player,IWorldEntity actor)
        {
            if (!Valid(player) || !QuestCsiInteraction.CanInteract(player,actor))
                return default;
            if (For(player) is not { } bar)
                return default;
            int mugIndex = Array.FindIndex(bar.Mugs,mug => Is(actor,mug.Site));
            if (mugIndex < 0 || Active(player,bar.Quest) is not { } quest)
                return default;
            int roundIndex = Array.FindIndex(bar.Rounds,round => Goal(quest,round.Goal,QuestObjectiveType.ActivateTargetGroup,round.Group) != null);
            if (roundIndex < 0)
                return default;
            Round current = bar.Rounds[roundIndex];
            Mug mug = bar.Mugs[mugIndex];
            uint bit = 1u << mugIndex;
            QuestObjective objective = Goal(quest,current.Goal,QuestObjectiveType.ActivateTargetGroup,current.Group);
            if (mugIndex >= current.Mugs || mug.Effects[roundIndex] == 0u || objective.HasChecklist(bit) ||
                GameTableManager.Instance.TargetGroup.GetEntry(current.Group) is not { Type: 1u } group ||
                !group.DataEntries.Contains(actor.CreatureId))
                return default;
            return (quest,objective,mug.Spell,mug.Csi,mug.Effects[roundIndex],bit);
        }

        internal static void CompleteCsi(IPlayer player,IWorldEntity actor)
        {
            var (quest,goal,_,_,_,bit) = PrepareCsi(player,actor);
            if (goal != null && goal.TryMarkChecklist(bit))
                quest.ObjectiveUpdate(goal.ObjectiveInfo.Id,1u);
        }

    }
}
