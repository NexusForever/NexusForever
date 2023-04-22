using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Movement.Spline;
using NexusForever.Game.Entity.Movement.Spline;
using NexusForever.Game.Static.Entity.Movement.Spline;
using NexusForever.Shared;

namespace NexusForever.Game.Entity.Movement
{
    public class GlobalMovementManager : Singleton<GlobalMovementManager>, IGlobalMovementManager
    {
        private delegate ISplineMode EntitySplineModeFactoryDelegate();
        private static ImmutableDictionary<SplineMode, EntitySplineModeFactoryDelegate> splineModeFactories;

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
