namespace NexusForever.Game.Abstract.Entity.Movement.Spline.Template
{
    public interface ISplineTemplateTbl : ISplineTemplate
    {
        /// <summary>
        /// Initialise spline template with spline id.
        /// </summary>
        void Initialise(ushort splineId);
    }
}
