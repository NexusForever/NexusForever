using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Loot;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Matching;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Game.Abstract.PublicEvent;
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
            IPlayerManager playerManager,
            ILootManager lootManager,
            IScriptManager scriptManager)
            : base(entityFactory, publicEventManager, playerManager, lootManager, scriptManager)
        {
        }

        #endregion

        protected override void InitialiseScriptCollection()
        {
            scriptCollection = scriptManager.InitialiseOwnedCollection<IContentPvpMapInstance>(this);
            scriptManager.InitialiseOwnedScripts<IContentPvpMapInstance>(scriptCollection, Entry.Id);
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

        /// <summary>
        /// Resurrect <see cref="IPlayer"/> with supplied <see cref="ResurrectionType"/>.
        /// </summary>
        public override void Resurrect(ResurrectionType type, IPlayer player)
        {
            if (Match is IPvpMatch pvpMatch)
            {
                switch (type)
                {
                    case ResurrectionType.Holocrypt:
                        OnResurrectHolocrypt(pvpMatch, player);
                        return;
                    default:
                        throw new InvalidOperationException();
                }
            }

            base.Resurrect(type, player);
        }

        private void OnResurrectHolocrypt(IPvpMatch pvpMatch, IPlayer player)
        {
            IMapEntrance entrance = pvpMatch.GetMapEntrance(player.Identity);

            player.Rotation = entrance.Rotation;
            player.TeleportToLocal(entrance.Position, false, (_) =>
            {
                pvpMatch.UpdatePool(player.Identity);

                player.ModifyHealth(player.MaxHealth, null, null);
                player.Shield = player.MaxShieldCapacity;
            });
        }

        /// <summary>
        /// Invoked when a <see cref="IPlayer"/> on the map dies.
        /// </summary>
        public override void OnDeath(IPlayer player)
        {
            if (Match is IPvpMatch pvpMatch)
                pvpMatch.OnDeath(player);
            else
                Resurrect(ResurrectionType.WakeHere, player);
        }
    }
}
