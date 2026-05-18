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
    public class PositionSplineCommand : IPositionCommand
    {
        public EntityCommand Command => EntityCommand.SetPositionSpline;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => spline?.IsFinialised ?? false;

        private ushort splineId;

        #region Dependency Injection

        private readonly ISplineTemplateTbl template;
        private readonly ISpline spline;

        public PositionSplineCommand(
            ISplineTemplateTbl template,
            ISpline spline)
        {
            this.template = template;
            this.spline = spline;
        }

        #endregion

        /// <summary>
        /// Initialise command with spline, <see cref="SplineType"/>, <see cref="SplineMode"/> and speed.
        /// </summary>
        public void Initialise(ushort splineId, SplineMode mode, float speed)
        {
            this.splineId = splineId;

            template.Initialise(splineId);
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
                Model   = new SetPositionSplineCommand
                {
                    SplineId            = splineId,
                    Speed               = spline.Speed,
                    Mode                = spline.Mode.Mode,
                    Position            = spline.Offset,
                    IsContinuing        = false,
                    AdjustSpeedToLength = true,
                    Blend               = false,
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
