using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using NexusForever.Shared;

namespace NexusForever.Network.World.Entity
{
    public sealed class EntityCommandManager : Singleton<EntityCommandManager>
    {
        private delegate IEntityCommandModel EntityCommandFactoryDelegate();
        private ImmutableDictionary<EntityCommand, EntityCommandFactoryDelegate> entityCommandFactories;
        private ImmutableDictionary<Type, EntityCommand> entityCommands;

        private EntityCommandManager()
        {
        }

        public void Initialise()
        {
            var factoryBuilder = ImmutableDictionary.CreateBuilder<EntityCommand, EntityCommandFactoryDelegate>();
            var commandBuilder = ImmutableDictionary.CreateBuilder<Type, EntityCommand>();

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
                commandBuilder.Add(type, attribute.Command);
            }

            entityCommandFactories = factoryBuilder.ToImmutable();
            entityCommands = commandBuilder.ToImmutable();
        }

        /// <summary>
        /// Return a new <see cref="IEntityCommandModel"/> of supplied <see cref="EntityCommand"/>.
        /// </summary>
        public IEntityCommandModel NewEntityCommand(EntityCommand command)
        {
            return entityCommandFactories.TryGetValue(command, out EntityCommandFactoryDelegate factory) ? factory.Invoke() : null;
        }

        /// <summary>
        /// Returns the <see cref="EntityCommand"/> for supplied <see cref="Type"/>.
        /// </summary>
        public EntityCommand? GetCommand(Type type)
        {
            if (entityCommands.TryGetValue(type, out EntityCommand command))
                return command;

            return null;
        }
    }
}
