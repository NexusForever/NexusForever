using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;

namespace NexusForever.Script.Template.Event
{
    public class EntitySummonEvent<T> : IEntitySummonEvent<T> where T : IWorldEntity
    {
        private IEntitySummonFactory factory;
        private IEntitySummonTemplate template;
        private Vector3 position;
        private Vector3 rotation;
        private OnAddDelegate add;

        public void Initialise(IEntitySummonFactory factory, IEntitySummonTemplate template, Vector3 position, Vector3 rotation, OnAddDelegate add = null)
        {
            this.factory  = factory;
            this.template = template;
            this.position = position;
            this.rotation = rotation;
            this.add      = add;
        }

        public void Invoke()
        {
            factory.Summon<T>(template, position, rotation, add);
        }
    }
}
