using System.Numerics;
using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Abstract.Map.Search
{
    public interface ISearchCheckRange<T> : ISearchCheck<T> where T : IGridEntity
    {
        void Initialise(Vector3 vector, float? radius);
    }
}
