using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Type;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Type
{
    public class SplineTypeFactory : ISplineTypeFactory
    {
        #region Dependency Injection

        private readonly IServiceProvider serviceProvider;

        public SplineTypeFactory(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        #endregion

        public ISplineType Create(SplineType type)
        {
            return type switch
            {
                SplineType.Linear     => serviceProvider.GetRequiredService<SplineTypeLinear>(),
                SplineType.CatmullRom => serviceProvider.GetRequiredService<SplineTypeCatmullRom>(),
                _                     => throw new NotImplementedException()
            };
        }
    }
}
