using NexusForever.Game.Static.Entity.Movement.Spline;
using System.Numerics;

namespace NexusForever.Game.Abstract.Entity.Movement.Spline.Template
{
    public interface ISplineTemplatePath : ISplineTemplate
    {
        /// <summary>
        /// Initialise spline template with supplied <see cref="SplineType"/> and points.
        /// </summary>
        void Initialise(SplineType type, List<Vector3> points);
    }
}
