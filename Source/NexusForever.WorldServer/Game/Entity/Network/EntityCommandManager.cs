using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

namespace NexusForever.WorldServer.Game.Entity.Network
{
    public static class EntityCommandManager
    {
        private delegate IEntityCommand EntityFactoryDelegate();
        private static ImmutableDictionary<EntityCommand, EntityFactoryDelegate> entityFactories;

        public static void Initialise()
        {
            var factories = new Dictionary<EntityCommand, EntityFactoryDelegate>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                IEnumerable<EntityCommandAttribute> attributes = type.GetCustomAttributes<EntityCommandAttribute>(false);
                foreach (EntityCommandAttribute attribute in attributes)
                {
                    NewExpression @new = Expression.New(type.GetConstructor(Type.EmptyTypes));
                    factories.Add(attribute.Command, Expression.Lambda<EntityFactoryDelegate>(@new).Compile());
                }
            }

            entityFactories = factories.ToImmutableDictionary();
        }

        public static IEntityCommand GetCommand(EntityCommand command)
        {
            return entityFactories.TryGetValue(command, out EntityFactoryDelegate factory) ? factory.Invoke() : null;
        }
    }
}
