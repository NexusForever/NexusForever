using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Prerequisite.Static;
using NexusForever.WorldServer.Game.Reputation.Static;

namespace NexusForever.WorldServer.Game.Prerequisite
{
    public sealed partial class PrerequisiteManager
    {
        [PrerequisiteCheck(PrerequisiteType.Level)]
        private static bool PrerequisiteCheckLevel(Player player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Level}!");
                    return true;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Race)]
        private static bool PrerequisiteCheckRace(Player player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.Race == (Race)value;
                case PrerequisiteComparison.NotEqual:
                    return player.Race != (Race)value;
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Race}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Class)]
        private static bool PrerequisiteCheckClass(Player player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.Class == (Class)value;
                case PrerequisiteComparison.NotEqual:
                    return player.Class != (Class)value;
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Class}!");
                    return true;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Quest)]
        private static bool PrerequisiteCheckQuest(Player player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal: // Active or Completed
                    return player.QuestManager.GetQuestState((ushort)objectId) == null;
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Quest}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Path)]
        private static bool PrerequisiteCheckPath(Player player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.PathManager.IsPathActive((Path)value);
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Path}!");

                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Achievement)]
        private static bool PrerequisiteCheckAchievement(Player player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.NotEqual:
                    return !player.AchievementManager.HasCompletedAchievement((ushort)objectId);
                case PrerequisiteComparison.Equal:
                    return player.AchievementManager.HasCompletedAchievement((ushort)objectId);
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Achievement}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.SpellBaseId)]
        private static bool PrerequisiteCheckSpellBaseId(Player player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.NotEqual:
                    return player.SpellManager.GetSpell(objectId) == null;
                case PrerequisiteComparison.Equal:
                    return player.SpellManager.GetSpell(objectId) != null;
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Achievement}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.BaseFaction)]
        private static bool PrerequisiteCheckBaseFaction(Player player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.Faction1 == (Faction)value;
                case PrerequisiteComparison.NotEqual:
                    return player.Faction1 != (Faction)value;
                default:
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Vital)]
        private static bool PrerequisiteCheckVital(Player player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                // TODO: Uncomment when Vitals are added ;)
                
                // case PrerequisiteComparison.Equal:
                //     return player.GetVitalValue((Vital)objectId) == value;
                // case PrerequisiteComparison.NotEqual:
                //     return player.GetVitalValue((Vital)objectId) != value;
                // case PrerequisiteComparison.GreaterThanOrEqual:
                //     return player.GetVitalValue((Vital)objectId) >= value;
                // case PrerequisiteComparison.GreaterThan:
                //     return player.GetVitalValue((Vital)objectId) > value;
                // case PrerequisiteComparison.LessThanOrEqual:
                //     return player.GetVitalValue((Vital)objectId) <= value;
                // case PrerequisiteComparison.LessThan:
                //     return player.GetVitalValue((Vital)objectId) < value;
                default:
                    log.Warn($"Unhandled {comparison} for {PrerequisiteType.Vital}!");
                    return false;
            }
        }
    }
}
