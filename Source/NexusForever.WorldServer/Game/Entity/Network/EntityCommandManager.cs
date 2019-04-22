using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

namespace NexusForever.WorldServer.Game.Entity.Network
{
    public static class EntityCommandManager
    {
        private delegate IEntityCommandModel EntityCommandFactoryDelegate();
        private static ImmutableDictionary<EntityCommand, EntityCommandFactoryDelegate> entityCommandFactories;
        private static ImmutableDictionary<Type, EntityCommand> entityCommands;

        public static void Initialise()
        {
            var factoryBuilder = ImmutableDictionary.CreateBuilder<EntityCommand, EntityCommandFactoryDelegate>();
            var commandBuilder = ImmutableDictionary.CreateBuilder<Type, EntityCommand>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                EntityCommandAttribute attribute = type.GetCustomAttribute<EntityCommandAttribute>();
                if (attribute == null)
                    continue;

                NewExpression @new = Expression.New(type.GetConstructor(Type.EmptyTypes));
                factoryBuilder.Add(attribute.Command, Expression.Lambda<EntityCommandFactoryDelegate>(@new).Compile());
                commandBuilder.Add(type, attribute.Command);
            }

            entityCommandFactories = factoryBuilder.ToImmutable();
            entityCommands = commandBuilder.ToImmutable();
        }

        /// <summary>
        /// Return a new <see cref="IEntityCommandModel"/> of supplied <see cref="EntityCommand"/>.
        /// </summary>
        public static IEntityCommandModel NewEntityCommand(EntityCommand command)
        {
            return entityCommandFactories.TryGetValue(command, out EntityCommandFactoryDelegate factory) ? factory.Invoke() : null;
        }

        /// <summary>
        /// Returns the <see cref="EntityCommand"/> for supplied <see cref="Type"/>.
        /// </summary>
        public static EntityCommand? GetCommand(Type type)
        {
            if (entityCommands.TryGetValue(type, out EntityCommand command))
                return command;

            return null;
        }
    }
}
