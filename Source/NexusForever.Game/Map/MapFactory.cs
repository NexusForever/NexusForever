using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Map.Instance;
using NexusForever.Game.Static.Map;
using NexusForever.Shared;

namespace NexusForever.Game.Map
{
    public class MapFactory : IMapFactory
    {
        #region Dependency Injection

        private readonly IFactoryInterface<IMap> factory;

        public MapFactory(
            IFactoryInterface<IMap> factory)
        {
            this.factory = factory;
        }

        #endregion

        public IMap CreateMap(MapType mapType)
        {
            switch (mapType)
            {
                case MapType.MiniDungeon:
                case MapType.Adventure:
                case MapType.Dungeon:
                    return factory.Resolve<ContentInstancedMap<IContentMapInstance>>();
                case MapType.Pvp:
                    return factory.Resolve<ContentInstancedMap<IContentPvpMapInstance>>();
                case MapType.Residence:
                case MapType.Community:
                    return factory.Resolve<ResidenceInstancedMap>();
                default:
                    return factory.Resolve<IBaseMap>();
            }
        }
    }
}
