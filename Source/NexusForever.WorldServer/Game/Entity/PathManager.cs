using NLog;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Network.Message.Model;
using System;

namespace NexusForever.WorldServer.Game.Entity
{
    public class PathManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Player player;
        private protected PathEntry pathEntry;

        public PathManager() { }

        /// <summary>
        /// Create a new <see cref="PathManager"/> from <see cref="Player"/> database model.
        /// </summary>
        public PathManager(Player owner, Character model)
        {
            player = owner;

            if(model.CharacterPath != null)
                pathEntry = new PathEntry(model.CharacterPath);
        }

        /// <summary>
        /// Create a new <see cref="PathEntry"/>.
        /// </summary>
        public PathEntry PathCreate(byte activePath, ulong amount = 0)
        {
            if (activePath < 0)
                return null;

            PathUnlocked pathUnlocked = CalculatePathUnlockedMask((Path)activePath);

            pathEntry = new PathEntry(
                player.CharacterId,
                (Path)activePath,
                pathUnlocked
            );
            return pathEntry;
        }

        /// <summary>
        /// Function to calculate <see cref="PathUnlocked"/> from <see cref="Path"/>
        /// </summary>
        /// <param name="activePath"></param>
        /// <returns></returns>
        public PathUnlocked CalculatePathUnlockedMask(Path activePath)
        {
            PathUnlocked pathUnlocked;
            switch ((Path)activePath)
            {
                case Path.Soldier:
                    pathUnlocked = PathUnlocked.Soldier;
                    break;
                case Path.Settler:
                    pathUnlocked = PathUnlocked.Settler;
                    break;
                case Path.Scientist:
                    pathUnlocked = PathUnlocked.Scientist;
                    break;
                case Path.Explorer:
                    pathUnlocked = PathUnlocked.Explorer;
                    break;
                default:
                    pathUnlocked = 0;
                    break;
            }

            return pathUnlocked;
        }

        /// <summary>
        /// Return a <see cref="Player"/>'s <see cref="PathEntry"/>. Create <see cref="Path.Soldier"/> entry if doesn't exist.
        /// </summary>
        /// <returns></returns>
        public PathEntry GetPathEntry()
        {
            if (pathEntry == null)
            {
                log.Warn($"No path associated with player {player.Name}. Defaulting to Soldier. ");
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
                throw new ArgumentException("No pathEntry exists. This should be called from the Player context.");

            return pathEntry.ActivePath;
        }

        /// <summary>
        /// /// Checks to see if a <see cref="Player"/>'s <see cref="Path"/> is active
        /// </summary>
        /// <param name="pathToCheck"></param>
        /// <returns></returns>
        public bool IsPathActive(Path pathToCheck)
        {
            if (pathEntry.ActivePath == pathToCheck)
                return true;

            return false;
        }

        /// <summary>
        /// Attempts to activate a <see cref="Player"/>'s <see cref="Path"/>
        /// </summary>
        /// <param name="pathToActivate"></param>
        /// <returns></returns>
        public void ActivatePath(Path pathToActivate)
        {
            if (IsPathUnlocked(pathToActivate))
            {
                if (IsPathActive(pathToActivate))
                    throw new ArgumentException("Path is already active.");

                pathEntry.ActivePath = pathToActivate;
                player.Path = pathEntry.ActivePath;
                SendServerPathActivateResult();
                SendSetUnitPathTypePacket();
                SendPathLogPacket();
            }
            else
            {
                throw new ArgumentException("Path is not unlocked.");
            }
        }

        /// <summary>
        /// Checks to see if a <see cref="Player"/>'s <see cref="Path"/> is mathced by a corresponding <see cref="PathUnlocked"/> flag
        /// </summary>
        /// <param name="pathToUnlock"></param>
        /// <returns></returns>
        public bool IsPathUnlocked(Path pathToUnlock)
        {
            PathUnlocked newPathMask = CalculatePathUnlockedMask(pathToUnlock);
            // Determines if the Path is already unlocked
            if ((pathEntry.PathsUnlocked & newPathMask) == newPathMask)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attemps to adjust the <see cref="Player"/>'s <see cref="PathUnlocked"/> status
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

            pathEntry.PathsUnlocked |= CalculatePathUnlockedMask(pathToUnlock);
            SendServerPathUnlockResult(Result);
            SendPathLogPacket();
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
            if(player == null)
                throw new ArgumentException("Needs player context to execute SendPathLogPacket()");

            player.Session.EnqueueMessageEncrypted(new ServerPathLog
            {
                ActivePath = pathEntry.ActivePath,
                PathProgress = new ServerPathLog.Progress
                {
                    Soldier = pathEntry.SoldierXp,
                    Settler = pathEntry.SettlerXp,
                    Scientist = pathEntry.ScientistXp,
                    Explorer = pathEntry.ExplorerXp
                },
                UnlockedPathMask = pathEntry.PathsUnlocked
            });
        }

        /// <summary>
        /// Used to tell the world (and the player) which Path Type this Player is.
        /// </summary>
        public void SendSetUnitPathTypePacket()
        {
            if (player == null)
                throw new ArgumentException("Needs player context to execute SendPathLogPacket()");

            player.Session.EnqueueMessageEncrypted(new ServerSetUnitPathType
            {
                Guid = player.Guid,
                Path = pathEntry.ActivePath,
            });
        }

        public void SendServerPathActivateResult(byte Result = 0)
        {
            if (player == null)
                throw new ArgumentException("Needs player context to execute SendPathLogPacket()");

            player.Session.EnqueueMessageEncrypted(new ServerPathActivateResult
            {
                Result = Result
            });
        }

        public void SendServerPathUnlockResult(byte Result = 0)
        {
            if (player == null)
                throw new ArgumentException("Needs player context to execute SendPathLogPacket()");

            player.Session.EnqueueMessageEncrypted(new ServerPathUnlockResult
            {
                Result = Result,
                UnlockedPathMask = pathEntry.PathsUnlocked
            });
        }

    }
}
