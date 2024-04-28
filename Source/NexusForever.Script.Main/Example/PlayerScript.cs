using NexusForever.Game.Abstract.Entity;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Example
{
    [ScriptFilterIgnore]
    public class PlayerScript : IPlayerScript, IOwnedScript<IPlayer>
    {
        private IPlayer owner;

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public void OnLoad(IPlayer owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Invoked on <see cref="IPlayer"/> login.
        /// </summary>
        public void OnLogin()
        {
        }

        /// <summary>
        /// Invoked on <see cref="IPlayer"/> logout.
        /// </summary>
        public void OnLogout()
        {
        }
    }
}
