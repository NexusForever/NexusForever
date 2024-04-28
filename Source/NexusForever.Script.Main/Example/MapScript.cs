using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Example
{
    [ScriptFilterOwnerId(870)]
    [ScriptFilterIgnore]
    public class MapScript : IMapScript, IOwnedScript<IBaseMap>
    {
        private IBaseMap owner;

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public void OnLoad(IBaseMap owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to map.
        /// </summary>
        public void OnAddToMap(IGridEntity entity)
        {
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is removed from map.
        /// </summary>
        public void OnRemoveFromMap(IGridEntity entity)
        {
        }
    }
}
