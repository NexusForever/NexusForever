using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;

namespace NexusForever.Script.Template.Event
{
    public interface IEntitySummonEvent<T> : IScriptEvent where T : IWorldEntity
    {
        void Initialise(IEntitySummonFactory factory, IEntitySummonTemplate template, Vector3 position, Vector3 rotation, OnAddDelegate add = null);
    }
}
