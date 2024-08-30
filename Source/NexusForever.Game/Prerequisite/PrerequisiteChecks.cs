﻿using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Game.Prerequisite
{
    public sealed partial class PrerequisiteManager
    {
        [PrerequisiteCheck(PrerequisiteType.Level)]
        private static bool PrerequisiteCheckLevel(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Level}!");
                    return true;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Race)]
        private static bool PrerequisiteCheckRace(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
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
        private static bool PrerequisiteCheckClass(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
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

        [PrerequisiteCheck(PrerequisiteType.QuestState)]
        private static bool PrerequisiteCheckQuestState(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.QuestManager.GetQuestState((ushort)objectId) == (QuestState)value;
                case PrerequisiteComparison.NotEqual:
                    return player.QuestManager.GetQuestState((ushort)objectId) != (QuestState)value;
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.QuestState}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.OtherPrerequisite)]
        private static bool PrerequisiteCheckPrerequisite(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.NotEqual:
                    return !Instance.Meets(player, objectId);
                case PrerequisiteComparison.Equal:
                    return Instance.Meets(player, objectId);
                default:
                    log.Warn($"Unhandled {comparison} for {PrerequisiteType.OtherPrerequisite}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Path)]
        private static bool PrerequisiteCheckPath(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
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

        [PrerequisiteCheck(PrerequisiteType.AchievementState)]
        private static bool PrerequisiteCheckAchievement(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.NotEqual:
                    return !player.AchievementManager.HasCompletedAchievement((ushort)objectId);
                case PrerequisiteComparison.Equal:
                    return player.AchievementManager.HasCompletedAchievement((ushort)objectId);
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.AchievementState}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.SpellBaseId)]
        private static bool PrerequisiteCheckSpellBaseId(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.NotEqual:
                    return player.SpellManager.GetSpell(objectId) == null;
                case PrerequisiteComparison.Equal:
                    return player.SpellManager.GetSpell(objectId) != null;
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.SpellBaseId}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.BaseFaction)]
        private static bool PrerequisiteCheckBaseFaction(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
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

        [PrerequisiteCheck(PrerequisiteType.PetFlair)]
        private static bool PrerequestCheckPetFlair(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.PetCustomisationManager.GetCustomisation(PetType.HoverBoard, objectId) != null;
                case PrerequisiteComparison.NotEqual:
                    return player.PetCustomisationManager.GetCustomisation(PetType.HoverBoard, objectId) == null;
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.PetFlair}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Vital)]
        private static bool PrerequisiteCheckVital(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
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

        [PrerequisiteCheck(PrerequisiteType.SpellObj)]
        private static bool PrerequisiteCheckSpellObj(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            // TODO: Confirm how the objectId is calculated. It seems like this check always checks for a Spell that is determined by an objectId.

            // Error message is "Spell requirement not met"

            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.SpellManager.GetSpell(value) != null;
                case PrerequisiteComparison.NotEqual:
                    return player.SpellManager.GetSpell(value) == null;
                default:
                    log.Warn($"Unhandled {comparison} for {PrerequisiteType.SpellObj}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.MountUsage)]
        private static bool PrerequisiteCheckUnknown194(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            // TODO: Only used in Mount check prerequisites. Its use is unknown.

            return true;
        }

        [PrerequisiteCheck(PrerequisiteType.HoverboardUsage)]
        private static bool PrerequisiteCheckUnknown195(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            // TODO: Only used in Mount check prerequisites. Its use is unknown.

            return true;
        }

        [PrerequisiteCheck(PrerequisiteType.Plane)]
        private static bool PrerequisiteCheckPlane(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            // Unknown how this works at this time, but there is a Spell Effect called "ChangePlane". Could be related.
            // TODO: Investigate further.

            // Returning true by default as many mounts used this
            return true;
        }

        [PrerequisiteCheck(PrerequisiteType.PurchasedTitle)]
        private static bool PrerequisiteCheckPurchasedTitle(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.TitleManager.HasTitle((ushort)objectId);
                case PrerequisiteComparison.NotEqual:
                    return !player.TitleManager.HasTitle((ushort)objectId);
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {(PrerequisiteType)288}!");
                    return true;
            }
        }
    }
}
