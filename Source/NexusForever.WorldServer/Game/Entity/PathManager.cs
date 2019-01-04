using System;
using System.Collections.Generic;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;
using NexusForever.WorldServer.Database;
using System.Collections;
using System.Linq;

namespace NexusForever.WorldServer.Game.Entity
{
    public class PathManager: ISaveCharacter, IEnumerable<PathEntry>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Player player;
        private readonly Dictionary<Path, PathEntry> paths = new Dictionary<Path, PathEntry>();

        /// <summary>
        /// Create a new <see cref="PathManager"/> from <see cref="Player"/> database model.
        /// </summary>
        public PathManager(Player owner, Character model)
        {
            player = owner;

            foreach (var pathEntry in model.CharacterPath)
                paths.Add((Path)Enum.Parse(typeof(Path), pathEntry.PathName), new PathEntry(pathEntry));
        }

        public void Load()
        {
            if (paths.Count <= 0)
                paths.TryAdd(player.Path, PathCreate(player.Path, true));

            if (paths.Count < 4)
                foreach (Path path in (Path[])Enum.GetValues(typeof(Path)))
                {
                    if (paths.ContainsKey(path))
                        continue;

                    paths.TryAdd(path, PathCreate(path));
                }

            // TODO: Check for missing level up rewards.

            SendPathLogPacket();
        }

        /// <summary>
        /// Create a new <see cref="PathEntry"/>.
        /// </summary>
        public PathEntry PathCreate(Path path, bool unlocked = false)
        {
            if (!Enum.IsDefined(typeof(Path), path))
                return null;

            if (paths.ContainsKey(path))
                throw new ArgumentException($"{path} is already added to the player!");

            PathEntry pathEntry = new PathEntry(
                player.CharacterId,
                path,
                unlocked
            );
            paths.Add(path, pathEntry);
            return pathEntry;
        }

        /// <summary>
        /// Return a <see cref="Player"/>'s <see cref="PathEntry"/>. Create <see cref="Path.Soldier"/> entry if doesn't exist.
        /// </summary>
        /// <returns></returns>
        public PathEntry GetPathEntry(Path path)
        {
            if (!paths.ContainsKey(path))
                throw new ArgumentException($"{path} is not added to the player!");

            return paths[path];
        }

        /// <summary>
        /// /// Checks to see if a <see cref="Player"/>'s <see cref="Path"/> is active
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
            if (!Enum.IsDefined(typeof(Path), pathToActivate))
                throw new ArgumentException("Path is not recognised.");

            if (!IsPathUnlocked(pathToActivate))
                throw new ArgumentException("Path is not unlocked.");
            
            if (IsPathActive(pathToActivate))
                throw new ArgumentException("Path is already active.");

            player.Path = pathToActivate;

            // TODO: Update activate timer

            SendServerPathActivateResult();
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
            return paths[pathToUnlock].Unlocked;
        }

        /// <summary>
        /// Attemps to adjust the <see cref="Player"/>'s <see cref="PathUnlockedMask"/> status
        /// </summary>
        /// <param name="pathToUnlock"></param>
        /// <returns></returns>
        public void UnlockPath(Path pathToUnlock)
        {
            byte Result = 0; // 0 == Ok
            if (!Enum.IsDefined(typeof(Path), pathToUnlock))
                throw new ArgumentException("Path is not recognised.");

            if (IsPathUnlocked(pathToUnlock))
                throw new ArgumentException("Path is already unlocked.");

            paths[pathToUnlock].Unlocked = true;

            SendServerPathUnlockResult(Result);
            SendPathLogPacket();
        }

        /// <summary>
        /// Add XP to the current <see cref="Path"/>
        /// </summary>
        /// <param name="xp"></param>
        public void AddXp(uint xp = 25)
        {
            if (xp <= 0)
                throw new ArgumentException("XP must be greater than 0.");

            Path path = player.Path;

            if (GetCurrentLevel(path) < 30)
            {
                paths[path].TotalXp += xp;

                uint[] newLevels = CheckForLevelUp(paths[path].TotalXp, xp);
                if (newLevels.Length > 0)
                    foreach (uint level in newLevels)
                        GrantLevelUpReward(path, level);

                SendServerPathUpdateXp(paths[path].TotalXp);
            }

            // TODO: Reward Elder XP after achieving rank 30
        }

        /// <summary>
        /// Get the current <see cref="Path"/> level for the <see cref="Player"/>
        /// </summary>
        /// <param name="path">The path being checked</param>
        /// <returns></returns>
        public uint GetCurrentLevel(Path path)
        {
            return Array.FindLast(GameTableManager.PathLevel.Entries, x => x.PathXP <= paths[path].TotalXp && x.PathTypeEnum == (uint)path).PathLevel;
        }

        /// <summary>
        /// Get the level based on an amount of XP
        /// </summary>
        /// <param name="xp">The XP value to get the level by</param>
        /// <returns></returns>
        private uint GetLevelByExperience(uint xp)
        {
            return Array.FindLast(GameTableManager.PathLevel.Entries, x => x.PathXP <= xp && x.PathTypeEnum == (uint)player.Path).PathLevel;
        }

        /// <summary>
        /// Check to see if a level up should happen based on current XP and XP just earned.
        /// </summary>
        /// <param name="totalXp">Path XP after XP earned has been applied</param>
        /// <param name="xpGained">XP just earned</param>
        /// <returns></returns>
        private uint[] CheckForLevelUp(uint totalXp, uint xpGained)
        {
            uint currentLevel = GetLevelByExperience(totalXp - xpGained);
            PathLevelEntry[] levelEntriesGained = Array.FindAll(GameTableManager.PathLevel.Entries, 
                x => x.PathLevel > currentLevel && 
                x.PathXP <= totalXp && 
                x.PathTypeEnum == (uint)player.Path
                );

            List<uint> levelsGained = new List<uint>();
            foreach (PathLevelEntry levelEntry in levelEntriesGained)
                levelsGained.Add(levelEntry.PathLevel);

            return levelsGained.ToArray();

        }

        /// <summary>
        /// Grants a player a level up reward for a <see cref="Path"/> and level
        /// </summary>
        /// <param name="path">The path to grant the reward for</param>
        /// <param name="level">The level to grant the reward for</param>
        private void GrantLevelUpReward(Path path, uint level)
        {
            Dictionary<Path, uint> baseRewardObjectId = new Dictionary<Path, uint>
            {
                { Path.Soldier, 7 },
                { Path.Settler, 37 },
                { Path.Scientist, 67 },
                { Path.Explorer, 97 }
            };
            uint pathRewardObjectId = baseRewardObjectId[path] + (Math.Clamp(level - 2, 0, 29)); // level - 2 is used because the objectIDs start at level 2 and a -2 offset was needed
            PathRewardEntry[] pathRewardEntries = Array.FindAll(GameTableManager.PathReward.Entries, x => x.ObjectId == pathRewardObjectId);

            foreach(PathRewardEntry pathRewardEntry in pathRewardEntries)
            {
                if (pathRewardEntry.PathRewardFlags > 0)
                    continue;

                if (pathRewardEntry.Item2Id == 0 && pathRewardEntry.Spell4Id == 0 && pathRewardEntry.CharacterTitleId == 0)
                    continue;

                if (pathRewardEntry.PrerequisiteId == 18 && player.Faction1 != Faction.Dominion)
                    continue;

                if (pathRewardEntry.PrerequisiteId == 19 && player.Faction1 != Faction.Exile)
                    continue;

                GrantPathReward(pathRewardEntry);
                paths[path].LevelRewarded = (byte)level;
                // TODO: Play Level up effect
                break;
            }
        }

        /// <summary>
        /// Grant the <see cref="Player"/> rewards from the <see cref="PathRewardEntry"/>
        /// </summary>
        /// <param name="pathRewardEntry">The entry containing items, spells, or titles, to be rewarded"/></param>
        private void GrantPathReward(PathRewardEntry pathRewardEntry)
        {
            log.Debug($"GrantPathReward Called, {pathRewardEntry.Id}");
            if (pathRewardEntry == null)
                throw new ArgumentNullException();

            // TODO: Check if there's bag space. Otherwise queue? Or is there an overflow inventory?
            if (pathRewardEntry.Item2Id > 0)
                player.Inventory.ItemCreate(pathRewardEntry.Item2Id, 1, 4);
            
            // TODO: Grant Spell rewards (needs PR #76)

            if (pathRewardEntry.CharacterTitleId > 0)
                player.TitleManager.AddTitle((ushort)pathRewardEntry.CharacterTitleId);

        }

        private PathUnlockedMask GetPathUnlockedMask()
        {
            PathUnlockedMask pathUnlockedMask = new PathUnlockedMask();

            if (paths[Path.Soldier].Unlocked)
                pathUnlockedMask |= PathUnlockedMask.Soldier;

            if (paths[Path.Settler].Unlocked)
                pathUnlockedMask |= PathUnlockedMask.Settler;

            if (paths[Path.Scientist].Unlocked)
                pathUnlockedMask |= PathUnlockedMask.Scientist;

            if (paths[Path.Explorer].Unlocked)
                pathUnlockedMask |= PathUnlockedMask.Explorer;

            return pathUnlockedMask;
        }

        /// <summary>
        /// Execute a DB Save of the <see cref="CharacterContext"/>
        /// </summary>
        /// <param name="context"></param>
        public void Save(CharacterContext context)
        {
            log.Debug($"PathManager.Save Called");
            foreach (PathEntry pathEntry in paths.Values)
                pathEntry.Save(context);
        }

        /// <summary>
        /// Used to update the Player's Path Log.
        /// </summary>
        public void SendPathLogPacket()
        {
            player.Session.EnqueueMessageEncrypted(new ServerPathLog
            {
                ActivePath = player.Path,
                PathProgress = new uint[]
                {
                    paths[Path.Soldier].TotalXp,
                    paths[Path.Settler].TotalXp,
                    paths[Path.Scientist].TotalXp,
                    paths[Path.Explorer].TotalXp
                },
                PathUnlockedMask = GetPathUnlockedMask(),
                ActivateTimer = 0 // TODO: Need to figure out timestamp calculations necessary for this value to update the client appropriately
            });
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
        /// <param name="Result">Used for success or error values</param>
        public void SendServerPathActivateResult(byte Result = 0)
        {

            player.Session.EnqueueMessageEncrypted(new ServerPathActivateResult
            {
                Result = Result
            });
        }

        /// <summary>
        /// Sends a response to the player's request for unlocking a <see cref="Path"/>
        /// </summary>
        /// <param name="Result">Used for success or error values</param>
        public void SendServerPathUnlockResult(byte Result = 0)
        {

            player.Session.EnqueueMessageEncrypted(new ServerPathUnlockResult
            {
                Result = Result,
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
