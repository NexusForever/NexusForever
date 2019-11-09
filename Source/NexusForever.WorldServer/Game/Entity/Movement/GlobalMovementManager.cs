using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using NexusForever.Shared;
using NexusForever.WorldServer.Game.Entity.Movement.Spline;
using NexusForever.WorldServer.Game.Entity.Movement.Spline.Implementation;
using NexusForever.WorldServer.Game.Entity.Movement.Spline.Static;

namespace NexusForever.WorldServer.Game.Entity.Movement
{
    public sealed class GlobalMovementManager : Singleton<GlobalMovementManager>
    {
        private delegate ISplineMode EntitySplineModeFactoryDelegate();
        private static ImmutableDictionary<SplineMode, EntitySplineModeFactoryDelegate> splineModeFactories;

        private GlobalMovementManager()
        {
        }

        public void Initialise()
        {
            InitialiseSplineModeFactories();
        }

        private void InitialiseSplineModeFactories()
        {
            var builder = ImmutableDictionary.CreateBuilder<SplineMode, EntitySplineModeFactoryDelegate>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                SplineModeAttribute attribute = type.GetCustomAttribute<SplineModeAttribute>();
                if (attribute == null)
                    continue;

                NewExpression @new = Expression.New(type.GetConstructor(Type.EmptyTypes));
                builder.Add(attribute.Mode, Expression.Lambda<EntitySplineModeFactoryDelegate>(@new).Compile());
            }

            splineModeFactories = builder.ToImmutable();
        }

        /// <summary>
        /// Return a new <see cref="ISplineMode"/> for supplied <see cref="SplineMode"/>.
        /// </summary>
        public ISplineMode NewSplineMode(SplineMode mode)
        {
            return splineModeFactories.TryGetValue(mode, out EntitySplineModeFactoryDelegate factory) ? factory.Invoke() : null;
        }
    }
}
