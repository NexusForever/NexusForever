using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Command.Position;
using NexusForever.Game.Abstract.Entity.Movement.Spline;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Template;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Game.Static.Entity.Movement.Spline;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;

namespace NexusForever.Game.Entity.Movement.Command.Position
{
    public class PositionPathCommand : IPositionCommand
    {
        public EntityCommand Command => EntityCommand.SetPositionPath;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => spline?.IsFinialised ?? false;

        #region Dependency Injection

        private readonly ISplineTemplatePath template;
        private readonly ISpline spline;

        public PositionPathCommand(
            ISplineTemplatePath template,
            ISpline spline)
        {
            this.template = template;
            this.spline   = spline;
        }

        #endregion

        /// <summary>
        /// Initialise command with the supplied nodes, <see cref="SplineType"/>, <see cref="SplineMode"/> and speed.
        /// </summary>
        public void Initialise(List<Vector3> nodes, SplineType type, SplineMode mode, float speed)
        {
            template.Initialise(type, nodes);
            spline.Initialise(template, mode, speed);
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            spline.Update(lastTick);
        }

        /// <summary>
        /// Returns the <see cref="INetworkEntityCommand"/> for the entity command.
        /// </summary>
        public INetworkEntityCommand GetNetworkEntityCommand()
        {
            return new NetworkEntityCommand
            {
                Command = Command,
                Model   = new SetPositionPathCommand
                {
                    Positions = spline.Points.Select(p => p.TemplatePoint.Position).ToList(),
                    Speed     = spline.Speed,
                    Type      = spline.Type.Type,
                    Mode      = spline.Mode.Mode,
                    Offset    = (uint)(spline.Position * 1000),
                    Blend     = false
                }
            };
        }

        /// <summary>
        /// Returns the current <see cref="Vector3"/> position for the entity command.
        /// </summary>
        public Vector3 GetPosition()
        {
            return spline.GetPosition();
        }

        /// <summary>
        /// Returns the current <see cref="Vector3"/> rotation for the entity command.
        /// </summary>
        /// <remarks>
        /// This is used to calculate the direction of travel for movement commands.
        /// </remarks>
        public Vector3 GetRotation()
        {
            return spline.GetRotation();
        }
    }
}
