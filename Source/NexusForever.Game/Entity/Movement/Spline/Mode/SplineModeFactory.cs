using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Mode;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Mode
{
    public class SplineModeFactory : ISplineModeFactory
    {
        #region Dependency Injection

        private readonly IServiceProvider serviceProvider;

        public SplineModeFactory(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        #endregion

        public ISplineMode Create(SplineMode mode)
        {
            return mode switch
            {
                SplineMode.OneShot             => serviceProvider.GetRequiredService<SplineModeOneShot>(),
                SplineMode.BackAndForth        => serviceProvider.GetRequiredService<SplineModeBackAndForth>(),
                SplineMode.Cyclic              => serviceProvider.GetRequiredService<SplineModeCyclic>(),
                SplineMode.OneShotReverse      => serviceProvider.GetRequiredService<SplineModeOneShotReverse>(),
                SplineMode.BackAndForthReverse => serviceProvider.GetRequiredService<SplineModeBackAndForthReverse>(),
                SplineMode.CyclicReverse       => serviceProvider.GetRequiredService<SplineModeCyclicReverse>(),
                _                              => throw new NotImplementedException()
            };
        }
    }
}
