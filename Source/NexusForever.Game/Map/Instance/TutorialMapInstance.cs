using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Loot;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.PublicEvent;
using NexusForever.Game.Static.Reputation;
using NexusForever.Script;

namespace NexusForever.Game.Map.Instance
{
    public class TutorialMapInstance : MapInstance, ITutorialMapInstance
    {
        /// <summary>
        /// <see cref="Faction"/> of the <see cref="ITutorialMapInstance"/>.
        /// </summary>
        public Faction Faction { get; private set; }

        #region Dependency Injection

        private readonly IScriptManager scriptManager;

        public TutorialMapInstance(
            IEntityFactory entityFactory,
            IPublicEventManager publicEventManager,
            IPlayerManager playerManager,
            ILootManager lootManager,
            IScriptManager scriptManager)
            : base(entityFactory, publicEventManager, playerManager, lootManager)
        {
            this.scriptManager = scriptManager;
        }

        #endregion

        protected override void InitialiseScriptCollection()
        {
            scriptCollection = scriptManager.InitialiseOwnedCollection<ITutorialMapInstance>(this);
            scriptManager.InitialiseOwnedScripts<ITutorialMapInstance>(scriptCollection, Entry.Id);
        }

        /// <summary>
        /// Initialise <see cref="ITutorialMapInstance"/> with <see cref="Faction"/>.
        /// </summary>
        public void Initialise(Faction faction)
        {
            Faction = faction;
        }

        protected override IMapPosition GetPlayerReturnLocation(IPlayer player)
        {
            throw new NotImplementedException();
        }
    }
}
