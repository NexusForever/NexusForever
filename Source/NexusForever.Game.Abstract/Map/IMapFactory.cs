using NexusForever.Game.Static.Map;

namespace NexusForever.Game.Abstract.Map
{
    public interface IMapFactory
    {
        IMap CreateMap(MapType mapType);
    }
}