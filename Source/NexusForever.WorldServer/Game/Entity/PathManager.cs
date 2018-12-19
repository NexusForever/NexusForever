using NLog;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Database.Character.Model;

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
        public PathEntry GetPath()
        {
            if (pathEntry == null)
            {
                log.Warn($"No path associated with player {player.Name}. Defaulting to Soldier. ");
                return PathCreate((byte)Path.Soldier);
            }

            return pathEntry;
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
        public bool ActivatePath(Path pathToActivate)
        {
            if (IsPathUnlocked(pathToActivate))
            {
                if (IsPathActive(pathToActivate))
                    return false;

                pathEntry.ActivePath = pathToActivate;
                return true;
            }

            return false;
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
        public bool UnlockPath(Path pathToUnlock)
        {
            if (pathToUnlock < 0)
                return false;

            if (IsPathUnlocked(pathToUnlock))
                return false;

            pathEntry.PathsUnlocked |= CalculatePathUnlockedMask(pathToUnlock);
            return true;
        }

        /// <summary>
        /// Execute a DB Save of the <see cref="CharacterContext"/>
        /// </summary>
        /// <param name="context"></param>
        public void Save(CharacterContext context)
        {
            pathEntry.Save(context);
        }

    }
}
