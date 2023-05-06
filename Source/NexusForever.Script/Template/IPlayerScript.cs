using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Script.Template
{
    public interface IPlayerScript : IUnitScript
    {
        /// <summary>
        /// Invoked on <see cref="IPlayer"/> login.
        /// </summary>
        void OnLogin()
        {
        }

        /// <summary>
        /// Invoked on <see cref="IPlayer"/> logout.
        /// </summary>
        void OnLogout()
        {
        }
    }
}
