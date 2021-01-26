using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Static;
using NexusForever.WorldServer.Game.Prerequisite;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    public class PathManager: ISaveCharacter, IEnumerable<PathEntry>
    {
        private const uint MaxPathCount = 4u;
        private const uint MaxPathLevel = 30u;

        private readonly Player player;
        private readonly Dictionary<Path, PathEntry> paths = new();

        /// <summary>
        /// Create a new <see cref="PathManager"/> from <see cref="Player"/> database model.
        /// </summary>
        public PathManager(Player owner, CharacterModel model)
        {
            player = owner;
            foreach (CharacterPathModel pathModel in model.Path)
                paths.Add((Path)pathModel.Path, new PathEntry(pathModel));

            Validate();
        }

        private void Validate()
        {
            if (paths.Count != MaxPathCount)
            {
                // sanity checks to make sure a player always has entries for all paths
                if (paths.Count == 0)
                    SetPathEntry(player.Path, PathCreate(player.Path, true));

                for (Path path = Path.Soldier; path <= Path.Explorer; path++)
                    if (GetPathEntry(path) == null)
                        SetPathEntry(path, PathCreate(path));
            }

            // TODO: Check for missing level up rewards.
        }

        /// <summary>
        /// Create a new <see cref="PathEntry"/>.
        /// </summary>
        private PathEntry PathCreate(Path path, bool unlocked = false)
        {
            if (path > Path.Explorer)
                return null;

            if (GetPathEntry(path) != null)
                throw new ArgumentException($"{path} is already added to the player!");

            var pathEntry = new PathEntry(
                player.CharacterId,
                path,
                unlocked
            );
            SetPathEntry(path, pathEntry);
            return pathEntry;
        }

        /// <summary>
        /// Checks to see if a <see cref="Player"/>'s <see cref="Path"/> is active
        /// </summary>
        /// <param name="pathToCheck"></param>
        /// <returns></returns>
        public bool IsPathActive(Path pathToCheck)
        {
            return player.Path == pathToCheck;
        }

        /// <summary>
        /// Attempts to activate a <see cref="Player"/>'s <see cref="Path"/>
        /// </summary>
        /// <param name="pathToActivate"></param>
        /// <returns></returns>
        public void ActivatePath(Path pathToActivate)
        {
            if (pathToActivate > Path.Explorer)
                throw new ArgumentException("Path is not recognised.");

            if (!IsPathUnlocked(pathToActivate))
                throw new ArgumentException("Path is not unlocked.");
            
            if (IsPathActive(pathToActivate))
                throw new ArgumentException("Path is already active.");

            player.Path = pathToActivate;

            SendServerPathActivateResult(GenericError.Ok);
            SendSetUnitPathTypePacket();
            SendPathLogPacket();
        }

        /// <summary>
        /// Checks to see if a <see cref="Player"/>'s <see cref="Path"/> is mathced by a corresponding <see cref="PathUnlockedMask"/> flag
        /// </summary>
        /// <param name="pathToUnlock"></param>
        /// <returns></returns>
        public bool IsPathUnlocked(Path pathToUnlock)
        {
            return GetPathEntry(pathToUnlock).Unlocked;
        }

        /// <summary>
        /// Attemps to adjust the <see cref="Player"/>'s <see cref="PathUnlockedMask"/> status
        /// </summary>
        /// <param name="pathToUnlock"></param>
        /// <returns></returns>
        public void UnlockPath(Path pathToUnlock)
        {
            if (pathToUnlock > Path.Explorer)
                throw new ArgumentException("Path is not recognised.");

            if (IsPathUnlocked(pathToUnlock))
                throw new ArgumentException("Path is already unlocked.");

            GetPathEntry(pathToUnlock).Unlocked = true;

            SendServerPathUnlockResult();
            SendPathLogPacket();
        }

        /// <summary>
        /// Add XP to the current <see cref="Path"/>
        /// </summary>
        /// <param name="xp"></param>
        public void AddXp(uint xp)
        {
            if (xp == 0)
                throw new ArgumentException("XP must be greater than 0.");

            Path path = player.Path;

            if (GetCurrentLevel(path) < MaxPathLevel)
            {
                PathEntry entry = GetPathEntry(path);

                checked
                {
                    entry.TotalXp += xp;
                }

                foreach (uint level in CheckForLevelUp(entry.TotalXp, xp))
                    GrantLevelUpReward(path, level);

                SendServerPathUpdateXp(entry.TotalXp);
            }

            // TODO: Reward Elder XP after achieving rank 30
        }

        /// <summary>
        /// Get the current <see cref="Path"/> level for the <see cref="Player"/>
        /// </summary>
        /// <param name="path">The path being checked</param>
        /// <returns></returns>
        private uint GetCurrentLevel(Path path)
        {
            return GameTableManager.Instance.PathLevel.Entries
                .Last(x => x.PathXP <= paths[path].TotalXp && x.PathTypeEnum == (uint)path).PathLevel;
        }

        /// <summary>
        /// Get the level based on an amount of XP
        /// </summary>
        /// <param name="xp">The XP value to get the level by</param>
        /// <returns></returns>
        private uint GetLevelByExperience(uint xp)
        {
            return GameTableManager.Instance.PathLevel.Entries
                .Last(x => x.PathXP <= xp && x.PathTypeEnum == (uint)player.Path).PathLevel;
        }

        /// <summary>
        /// Check to see if a level up should happen based on current XP and XP just earned.
        /// </summary>
        /// <param name="totalXp">Path XP after XP earned has been applied</param>
        /// <param name="xpGained">XP just earned</param>
        /// <returns></returns>
        private IEnumerable<uint> CheckForLevelUp(uint totalXp, uint xpGained)
        {
            uint currentLevel = GetLevelByExperience(totalXp - xpGained);
            return GameTableManager.Instance.PathLevel.Entries
                .Where(x => x.PathLevel > currentLevel && x.PathXP <= totalXp && x.PathTypeEnum == (uint)player.Path)
                .Select(e => e.PathLevel);
        }

        /// <summary>
        /// Grants a player a level up reward for a <see cref="Path"/> and level
        /// </summary>
        /// <param name="path">The path to grant the reward for</param>
        /// <param name="level">The level to grant the reward for</param>
        private void GrantLevelUpReward(Path path, uint level)
        {
            // TODO: look at this in more in depth, might be a better way to handle
            uint baseRewardObjectId = (uint)path * MaxPathLevel + 7u; // 7 is the base offset
            uint pathRewardObjectId = baseRewardObjectId + (Math.Clamp(level - 2, 0, 29)); // level - 2 is used because the objectIDs start at level 2 and a -2 offset was needed

            IEnumerable<PathRewardEntry> pathRewardEntries = GameTableManager.Instance.PathReward.Entries
                .Where(x => x.ObjectId == pathRewardObjectId);
            foreach (PathRewardEntry pathRewardEntry in pathRewardEntries)
            {
                if (pathRewardEntry.PathRewardFlags > 0)
                    continue;

                if (pathRewardEntry.PathRewardTypeEnum != 0)
                    continue;

                if (pathRewardEntry.Item2Id == 0 && pathRewardEntry.Spell4Id == 0 && pathRewardEntry.CharacterTitleId == 0)
                    continue;

                if (pathRewardEntry.PrerequisiteId > 0 && !PrerequisiteManager.Instance.Meets(player, pathRewardEntry.PrerequisiteId))
                    continue;

                GrantPathReward(pathRewardEntry);
            }

            GetPathEntry(path).LevelRewarded = (byte)level;
            player.CastSpell(53234, new Spell.SpellParameters());
        }

        /// <summary>
        /// Grant the <see cref="Player"/> rewards from the <see cref="PathRewardEntry"/>
        /// </summary>
        /// <param name="pathRewardEntry">The entry containing items, spells, or titles, to be rewarded"/></param>
        private void GrantPathReward(PathRewardEntry pathRewardEntry)
        {
            if (pathRewardEntry == null)
                throw new ArgumentNullException();

            // TODO: Check if there's bag space. Otherwise queue? Or is there an overflow inventory?
            if (pathRewardEntry.Item2Id > 0)
                player.Inventory.ItemCreate(pathRewardEntry.Item2Id, 1, ItemUpdateReason.PathReward);

            if (pathRewardEntry.Spell4Id > 0)
            {
                Spell4Entry spell4Entry = GameTableManager.Instance.Spell4.GetEntry(pathRewardEntry.Spell4Id);
                player.SpellManager.AddSpell(spell4Entry.Spell4BaseIdBaseSpell);
            }

            if (pathRewardEntry.CharacterTitleId > 0)
                player.TitleManager.AddTitle((ushort)pathRewardEntry.CharacterTitleId);

        }

        private PathUnlockedMask GetPathUnlockedMask()
        {
            PathUnlockedMask mask = PathUnlockedMask.None;
            foreach (PathEntry entry in paths.Values)
                if (entry.Unlocked)
                    mask |= (PathUnlockedMask)(1 << (int)entry.Path);

            return mask;
        }

        /// <summary>
        /// Execute a DB Save of the <see cref="CharacterContext"/>
        /// </summary>
        /// <param name="context"></param>
        public void Save(CharacterContext context)
        {
            foreach (PathEntry pathEntry in paths.Values)
                pathEntry.Save(context);
        }

        public void SendInitialPackets()
        {
            SendPathLogPacket();
        }

        /// <summary>
        /// Used to update the Player's Path Log.
        /// </summary>
        private void SendPathLogPacket()
        {
            player.Session.EnqueueMessageEncrypted(new ServerPathLog
            {
                ActivePath = player.Path,
                PathProgress = paths.Values.Select(p => p.TotalXp).ToArray(),
                PathUnlockedMask = GetPathUnlockedMask(),
                TimeSinceLastActivateInDays = GetCooldownTime() // TODO: Need to figure out timestamp calculations necessary for this value to update the client appropriately
            });
        }

        private float GetCooldownTime()
        {
            return (float)DateTime.UtcNow.Subtract(player.PathActivatedTime).TotalDays * -1;
        }

        /// <summary>
        /// Used to tell the world (and the player) which Path Type this Player is.
        /// </summary>
        public void SendSetUnitPathTypePacket()
        {
            player.EnqueueToVisible(new ServerSetUnitPathType
            {
                Guid = player.Guid,
                Path = player.Path,
            }, true);
        }

        /// <summary>
        /// Sends a response to the player's <see cref="Path"/> activate request
        /// </summary>
        /// <param name="result">Used for success or error values</param>
        public void SendServerPathActivateResult(GenericError result = GenericError.Ok)
        {
            player.Session.EnqueueMessageEncrypted(new ServerPathActivateResult
            {
                Result = result
            });
        }

        /// <summary>
        /// Sends a response to the player's request for unlocking a <see cref="Path"/>
        /// </summary>
        /// <param name="result">Used for success or error values</param>
        public void SendServerPathUnlockResult(GenericError result = GenericError.Ok)
        {
            player.Session.EnqueueMessageEncrypted(new ServerPathUnlockResult
            {
                Result = result,
                UnlockedPathMask = GetPathUnlockedMask()
            });
        }

        /// <summary>
        /// Sends total XP for the activate path to the player
        /// </summary>
        /// <param name="totalXp">Total Path XP to be sent</param>
        private void SendServerPathUpdateXp(uint totalXp)
        {
            player.Session.EnqueueMessageEncrypted(new ServerPathUpdateXP
            {
                TotalXP = totalXp
            });
        }

        private PathEntry GetPathEntry(Path path)
        {
            paths.TryGetValue(path, out PathEntry pathEntry);
            return pathEntry;
        }

        private void SetPathEntry(Path path, PathEntry entry)
        {
            paths[path] = entry;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<PathEntry> GetEnumerator()
        {
            return paths.Values.GetEnumerator();
        }
    }
}
