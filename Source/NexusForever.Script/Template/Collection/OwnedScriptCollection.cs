using Microsoft.Extensions.Logging;

namespace NexusForever.Script.Template.Collection
{
    public class OwnedScriptCollection<T> : ScriptCollection, IOwnedScriptCollection<T> where T : class
    {
        private T owner;

        #region Dependency Injection

        public OwnedScriptCollection(
            ILogger<IScriptCollection> log,
            IScriptManager scriptManager)
            : base(log, scriptManager)
        {
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IOwnedScriptCollection{T}"/> with supplied <typeparamref name="T"/> owner.
        /// </summary>
        public void Initialise(T owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Load supplied <see cref="IScriptInstanceInfo"/> into <see cref="IOwnedScriptCollection{T}"/>.
        /// </summary>
        public override void Load(IScriptInstanceInfo instanceInfo)
        {
            base.Load(instanceInfo);

            if (instanceInfo.Script is IOwnedScript<T> ownedScript)
                ownedScript.OnLoad(owner);
        }
    }
}
