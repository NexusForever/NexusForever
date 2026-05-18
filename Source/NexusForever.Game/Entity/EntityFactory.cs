using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Entity
{
    public class EntityFactory : IEntityFactory
    {
        #region Dependency Injection

        private readonly IServiceProvider serviceProvider;

        public EntityFactory(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        #endregion

        /// <summary>
        /// Create a new entity of type <typeparamref name="T"/>.
        /// </summary>
        public T CreateEntity<T>() where T : IGridEntity
        {
            return serviceProvider.GetService<T>();
        }

        /// <summary>
        /// Create a new <see cref="IWorldEntity"/> of supplied <see cref="EntityType"/>.
        /// </summary>
        /// <remarks>
        /// This should only be used for creating entities from the database, otherwise use <see cref="CreateEntity{T}"/>.
        /// </remarks>
        public IWorldEntity CreateWorldEntity(EntityType type)
        {
            return type switch
            {
                EntityType.NonPlayer          => serviceProvider.GetService<INonPlayerEntity>(),
                EntityType.Chest              => serviceProvider.GetService<IChestEntity>(),
                EntityType.Destructible       => serviceProvider.GetService<IDestructibleEntity>(),
                EntityType.Vehicle            => serviceProvider.GetService<IVehicleEntity>(),
                EntityType.Door               => serviceProvider.GetService<IDoorEntity>(),
                EntityType.HarvestUnit        => serviceProvider.GetService<IHarvestUnitEntity>(),
                EntityType.CorpseUnit         => serviceProvider.GetService<ICorpseUnitEntity>(),
                EntityType.Mount              => serviceProvider.GetService<IMountEntity>(),
                EntityType.CollectableUnit    => serviceProvider.GetService<ICollectableUnitEntity>(),
                EntityType.Taxi               => serviceProvider.GetService<ITaxiEntity>(),
                EntityType.Simple             => serviceProvider.GetService<ISimpleEntity>(),
                EntityType.Platform           => serviceProvider.GetService<IPlatformEntity>(),
                EntityType.MailBox            => serviceProvider.GetService<IMailboxEntity>(),
                EntityType.AiTurret           => serviceProvider.GetService<IAiTurretEntity>(),
                EntityType.InstancePortal     => serviceProvider.GetService<IInstancePortalEntity>(),
                EntityType.Plug               => serviceProvider.GetService<IPlugEntity>(),
                EntityType.Residence          => serviceProvider.GetService<IResidenceEntity>(),
                EntityType.StructuredPlug     => serviceProvider.GetService<IStructuredPlugEntity>(),
                EntityType.PinataLoot         => serviceProvider.GetService<IPinataLootEntity>(),
                EntityType.BindPoint          => serviceProvider.GetService<IBindPointEntity>(),
                EntityType.Player             => serviceProvider.GetService<IPlayer>(),
                EntityType.Hidden             => serviceProvider.GetService<IHiddenEntity>(),
                EntityType.Trigger            => serviceProvider.GetService<ITriggerEntity>(),
                EntityType.Ghost              => serviceProvider.GetService<IGhostEntity>(),
                EntityType.Pet                => serviceProvider.GetService<IPetEntity>(),
                EntityType.EsperPet           => serviceProvider.GetService<IEsperPetEntity>(),
                EntityType.WorldUnit          => serviceProvider.GetService<IWorldUnitEntity>(),
                EntityType.ScannerUnit        => serviceProvider.GetService<IScannerUnitEntity>(),
                EntityType.Camera             => serviceProvider.GetService<ICameraEntity>(),
                EntityType.Trap               => serviceProvider.GetService<ITrapEntity>(),
                EntityType.DestructibleDoor   => serviceProvider.GetService<IDestructibleDoorEntity>(),
                EntityType.Pickup             => serviceProvider.GetService<IPickupEntity>(),
                EntityType.SimpleCollidable   => serviceProvider.GetService<ISimpleCollidableEntity>(),
                EntityType.HousingMannequin   => serviceProvider.GetService<IHousingMannequinEntity>(),
                EntityType.HousingHarvestPlug => serviceProvider.GetService<IHousingHarvestPlugEntity>(),
                EntityType.HousingPlant       => serviceProvider.GetService<IHousingPlantEntity>(),
                EntityType.Lockbox            => serviceProvider.GetService<ILockboxEntity>(),
                _                             => throw new NotImplementedException()
            };
        }
    }
}
