using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Event;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Matching;
using NexusForever.Script;
using NexusForever.Script.Template;

namespace NexusForever.Game.Map.Instance
{
    public class ContentPvpMapInstance : ContentMapInstance, IContentPvpMapInstance
    {
        #region Dependency Injection

        public ContentPvpMapInstance(
            IEntityFactory entityFactory,
            IPublicEventManager publicEventManager,
            IScriptManager scriptManager)
            : base(entityFactory, publicEventManager, scriptManager)
        {
        }

        #endregion

        protected override void InitialiseScriptCollection()
        {
            scriptCollection = scriptManager.InitialiseOwnedScripts<IContentPvpMapInstance>(this, Entry.Id);
        }

        /// <summary>
        /// Invoked when the <see cref="IPvpMatch"/> for the map changes <see cref="PvpGameState"/>.
        /// </summary>
        public void OnPvpMatchState(PvpGameState state)
        {
            scriptCollection.Invoke<IContentPvpMapScript>(s => s.OnPvpMatchState(state));
        }

        /// <summary>
        /// Invoked when the <see cref="IPvpMatch"/> for the map finishes.
        /// </summary>
        public void OnPvpMatchFinish(MatchWinner matchWinner, MatchEndReason matchEndReason)
        {
            scriptCollection.Invoke<IContentPvpMapScript>(s => s.OnPvpMatchFinish(matchWinner, matchEndReason));
        }

        /// <summary>
        /// Return <see cref="ResurrectionType"/> applicable to this map.
        /// </summary>
        public override ResurrectionType GetResurrectionType()
        {
            return ResurrectionType.Holocrypt;
        }
    }
}
