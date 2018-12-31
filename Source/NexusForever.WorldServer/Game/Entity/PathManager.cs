using System;
using System.Collections.Generic;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Game.Entity
{
    public class PathManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Player player;
        private protected PathEntry pathEntry;

        /// <summary>
        /// Create a new <see cref="PathManager"/> from <see cref="Player"/> database model.
        /// </summary>
        public PathManager(Player owner, Character model)
        {
            player = owner;

            if (model.CharacterPath != null)
                pathEntry = new PathEntry(model.CharacterPath);
        }

        /// <summary>
        /// Create a new <see cref="PathEntry"/>.
        /// </summary>
        public PathEntry PathCreate(byte activePath, ulong amount = 0)
        {
            if (activePath < 0)
                return null;

            pathEntry = new PathEntry(
                player.CharacterId,
                (Path)activePath,
                (PathUnlockedMask)(1 << activePath)
            );
            return pathEntry;
        }

        /// <summary>
        /// Return a <see cref="Player"/>'s <see cref="PathEntry"/>. Create <see cref="Path.Soldier"/> entry if doesn't exist.
        /// </summary>
        /// <returns></returns>
        public PathEntry GetPathEntry()
        {
            if (pathEntry == null)
            {
                log.Warn($"No path associated with player {player.Name}. Defaulting to Soldier.");
                return PathCreate((byte)Path.Soldier);
            }

            return pathEntry;
        }

        /// <summary>
        /// Returns the <see cref="Path"/> for this <see cref="Player"/>
        /// </summary>
        /// <returns></returns>
        public Path GetPath()
        {
            if (pathEntry == null)
            {
                log.Warn($"No path associated with player {player.Name}. Defaulting to Soldier.");
                return PathCreate((byte)Path.Soldier).ActivePath;
            }

            return pathEntry.ActivePath;
        }

        /// <summary>
        /// /// Checks to see if a <see cref="Player"/>'s <see cref="Path"/> is active
        /// </summary>
        /// <param name="pathToCheck"></param>
        /// <returns></returns>
        public bool IsPathActive(Path pathToCheck)
        {
            return pathEntry.ActivePath == pathToCheck;
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

            pathEntry.ActivePath = pathToActivate;
            player.Path = pathEntry.ActivePath;
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
            PathUnlockedMask newPathMask = (PathUnlockedMask)(1 << (int)pathToUnlock);
            // Determines if the Path is already unlocked
            return (pathEntry.PathsUnlocked & newPathMask) == newPathMask;
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

            pathEntry.PathsUnlocked |= (PathUnlockedMask)(1 << (int)pathToUnlock);
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

            uint[] newLevels = new uint[0];
            switch (pathEntry.ActivePath)
            {
                case Path.Soldier:
                    pathEntry.SoldierXp += xp;
                    SendServerPathUpdateXp(pathEntry.SoldierXp);

                    newLevels = CheckForLevelUp(pathEntry.SoldierXp, xp);
                    if (newLevels.Length > 0)
                        foreach (uint level in newLevels)
                            GrantLevelUpReward(pathEntry.ActivePath, level);
                    break;
                case Path.Settler:
                    pathEntry.SettlerXp += xp;
                    SendServerPathUpdateXp(pathEntry.SettlerXp);

                    newLevels = CheckForLevelUp(pathEntry.SettlerXp, xp);
                    if (newLevels.Length > 0)
                        foreach (uint level in newLevels)
                            GrantLevelUpReward(pathEntry.ActivePath, level);
                    break;
                case Path.Scientist:
                    pathEntry.ScientistXp += xp;
                    SendServerPathUpdateXp(pathEntry.ScientistXp);

                    newLevels = CheckForLevelUp(pathEntry.ScientistXp, xp);
                    if (newLevels.Length > 0)
                        foreach (uint level in newLevels)
                            GrantLevelUpReward(pathEntry.ActivePath, level);
                    break;
                case Path.Explorer:
                    pathEntry.ExplorerXp += xp;
                    SendServerPathUpdateXp(pathEntry.ExplorerXp);

                    newLevels = CheckForLevelUp(pathEntry.ExplorerXp, xp);
                    if (newLevels.Length > 0)
                        foreach (uint level in newLevels)
                            GrantLevelUpReward(pathEntry.ActivePath, level);
                    break;
                default:
                    throw new ArgumentException($"Path not recognised: {pathEntry.ActivePath}");
            }
        }

        /// <summary>
        /// Get the current <see cref="Path"/> level for the <see cref="Player"/>
        /// </summary>
        /// <param name="path">The path being checked</param>
        /// <returns></returns>
        public uint GetCurrentLevel(Path path)
        {
            Dictionary<Path, uint> pathXpValue = new Dictionary<Path, uint>
            {
                { Path.Soldier, pathEntry.SoldierXp },
                { Path.Settler, pathEntry.SettlerXp },
                { Path.Scientist, pathEntry.ScientistXp },
                { Path.Explorer, pathEntry.ExplorerXp }
            };
            return Array.FindLast(GameTableManager.PathLevel.Entries, x => x.PathXP <= pathXpValue[path] && x.PathTypeEnum == (uint)path).PathLevel;
        }

        /// <summary>
        /// Get the level based on an amount of XP
        /// </summary>
        /// <param name="xp">The XP value to get the level by</param>
        /// <returns></returns>
        private uint GetLevelByExperience(uint xp)
        {
            return Array.FindLast(GameTableManager.PathLevel.Entries, x => x.PathXP <= xp && x.PathTypeEnum == (uint)pathEntry.ActivePath).PathLevel;
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
                x.PathTypeEnum == (uint)pathEntry.ActivePath
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
            Dictionary<Path, Action> PathActions = new Dictionary<Path, Action>
            {
                { Path.Soldier, () => pathEntry.SoldierLevelRewarded = level },
                { Path.Settler, () => pathEntry.SettlerLevelRewarded = level },
                { Path.Scientist, () => pathEntry.ScientistLevelRewarded = level },
                { Path.Explorer, () => pathEntry.ExplorerLevelRewarded = level }
            };
            uint pathRewardObjectId = baseRewardObjectId[path] + (Math.Clamp(level - 2, 0, 100)); // level - 2 is used because the objectIDs start at level 2 and a -2 offset was needed
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
                PathActions[path]();
                break;
            }
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
                player.Inventory.ItemCreate(pathRewardEntry.Item2Id, 1, 4);
            // TODO: Grant Spell rewards (needs PR #76)
            // TODO: Grant Title rewards (needs PR #64)
        }

        /// <summary>
        /// Execute a DB Save of the <see cref="CharacterContext"/>
        /// </summary>
        /// <param name="context"></param>
        public void Save(CharacterContext context)
        {
            pathEntry.Save(context);
        }

        /// <summary>
        /// Used to update the Player's Path Log.
        /// </summary>
        public void SendPathLogPacket()
        {
            player.Session.EnqueueMessageEncrypted(new ServerPathLog
            {
                ActivePath = pathEntry.ActivePath,
                PathProgress = new uint[]
                {
                    pathEntry.SoldierXp,
                    pathEntry.SettlerXp,
                    pathEntry.ScientistXp,
                    pathEntry.ExplorerXp
                },
                PathUnlockedMask = pathEntry.PathsUnlocked,
                ActivateTimer = 0 // TODO: Need to figure out timestamp calculations necessary for this value to update the client appropriately
            });
        }

        /// <summary>
        /// Used to tell the world (and the player) which Path Type this Player is.
        /// </summary>
        public void SendSetUnitPathTypePacket()
        {

            player.Session.EnqueueMessageEncrypted(new ServerSetUnitPathType
            {
                Guid = player.Guid,
                Path = pathEntry.ActivePath,
            });
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
                UnlockedPathMask = pathEntry.PathsUnlocked
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
    }
}
