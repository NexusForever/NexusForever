using NexusForever.IO.Map;

namespace NexusForever.Game.Abstract.Map
{
    public interface IMapIOManager
    {
        void Initialise();

        /// <summary>
        /// Returns an existing <see cref="MapFile"/> for the supplied asset, if it doesn't exist a new one will be created from disk.
        /// </summary>
        MapFile GetBaseMap(string assetPath);
    }
}