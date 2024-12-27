using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Entity.Movement.Spline;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Mode;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Template;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Type;
using NexusForever.Game.Entity.Movement.Spline.Mode;
using NexusForever.Game.Entity.Movement.Spline.Template;
using NexusForever.Game.Entity.Movement.Spline.Type;

namespace NexusForever.Game.Entity.Movement.Spline
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameEntityMovementSpline(this IServiceCollection sc)
        {
            sc.AddTransient<ISplineTemplatePath, SplineTemplatePath>();
            sc.AddTransient<ISplineTemplateTbl, SplineTemplateTbl>();
            sc.AddTransient<ISpline, Spline>();

            sc.AddTransient<ISplineTypeFactory, SplineTypeFactory>();
            sc.AddTransient<SplineTypeCatmullRom>();
            sc.AddTransient<SplineTypeLinear>();

            sc.AddTransient<ISplineModeFactory, SplineModeFactory>();
            sc.AddTransient<SplineModeOneShot>();
            sc.AddTransient<SplineModeBackAndForth>();
            sc.AddTransient<SplineModeCyclic>();
            sc.AddTransient<SplineModeOneShotReverse>();
            sc.AddTransient<SplineModeBackAndForthReverse>();
            sc.AddTransient<SplineModeCyclicReverse>();
        }
    }
}
