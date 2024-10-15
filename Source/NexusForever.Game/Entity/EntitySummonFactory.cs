using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;

namespace NexusForever.Game.Entity
{
    public class EntitySummonFactory : IEntitySummonFactory
    {
        public uint SummonCount => (uint)summonGuids.Count;

        public event Action<uint> OnSummon;
        public event Action<uint> OnUnsummon;

        private IWorldEntity owner;

        private readonly HashSet<uint> summonGuids = [];

        #region Dependency Injection

        private readonly IEntityFactory entityFactory;

        public EntitySummonFactory(
            IEntityFactory entityFactory)
        {
            this.entityFactory = entityFactory;
        }

        #endregion

        /// <summary>
        /// Initialises the factory with the owner of the summons.
        /// </summary>
        public void Initialise(IWorldEntity owner)
        {
            if (this.owner != null)
                throw new InvalidOperationException();

            this.owner = owner;
        }

        /// <summary>
        /// Summons an entity of <typeparamref name="T"/> at the specified position and rotation and optional callback.
        /// </summary>
        public void Summon<T>(IEntitySummonTemplate template, Vector3 position, Vector3 rotation, OnAddDelegate add = null) where T : IWorldEntity
        {
            var entity = entityFactory.CreateEntity<T>();
            entity.Initialise(template.CreatureId);
            entity.DisplayInfo  = template.DisplayInfoId;
            entity.Faction1     = template.Faction;
            entity.Faction2     = template.Faction;
            entity.Rotation     = rotation;
            entity.SummonerGuid = owner.Guid;

            Summon(entity, position, add);
        }

        private void Summon<T>(T entity, Vector3 position, OnAddDelegate add) where T : IWorldEntity
        {
            if (add != null)
            {
                entity.AddToMap(owner.Map, position, (map, guid, vector) =>
                {
                    TrackSummon(map, guid, vector);
                    add(map, guid, vector);
                });
            }
            else
                entity.AddToMap(owner.Map, position, TrackSummon);
        }

        private void TrackSummon(IBaseMap map, uint guid, Vector3 position)
        {
            summonGuids.Add(guid);
            OnSummon?.Invoke(guid);
        }

        /// <summary>
        /// Stop tracking a summon.
        /// </summary>
        /// <remarks>
        /// This should be called when a summon is removed from the world.
        /// </remarks>
        public void UntrackSummon(uint guid)
        {
            summonGuids.Remove(guid);
            OnUnsummon?.Invoke(guid);
        }

        /// <summary>
        /// Unsummon an summoned entity with supplied guid.
        /// </summary>
        public void Unsummon(uint guid)
        {
            if (!summonGuids.Contains(guid))
                return;

            UntrackSummon(guid);

            var summon = owner.Map.GetEntity<INonPlayerEntity>(guid);
            if (summon == null)
                return;

            summon.RemoveFromMap();
        }

        /// <summary>
        /// Unsummon all summoned entities.
        /// </summary>
        public void Unsummon()
        {
            foreach (uint guid in summonGuids)
                Unsummon(guid);

            summonGuids.Clear();
        }
    }
}
