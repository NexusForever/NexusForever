using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Shared;

namespace NexusForever.Network.World.Entity
{
    public sealed class EntityCommandManager : Singleton<EntityCommandManager>, IEntityCommandManager
    {
        private delegate IEntityCommandModel EntityCommandFactoryDelegate();
        private ImmutableDictionary<EntityCommand, EntityCommandFactoryDelegate> entityCommandFactories;

        public void Initialise()
        {
            var factoryBuilder = ImmutableDictionary.CreateBuilder<EntityCommand, EntityCommandFactoryDelegate>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                EntityCommandAttribute attribute = type.GetCustomAttribute<EntityCommandAttribute>();
                if (attribute == null)
                    continue;

                ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor == null)
                    continue;

                NewExpression @new = Expression.New(constructor);
                factoryBuilder.Add(attribute.Command, Expression.Lambda<EntityCommandFactoryDelegate>(@new).Compile());
            }

            entityCommandFactories = factoryBuilder.ToImmutable();
        }

        /// <summary>
        /// Return a new <see cref="IEntityCommandModel"/> of supplied <see cref="EntityCommand"/>.
        /// </summary>
        public IEntityCommandModel NewEntityCommand(EntityCommand command)
        {
            return entityCommandFactories.TryGetValue(command, out EntityCommandFactoryDelegate factory) ? factory.Invoke() : null;
        }
    }
}
