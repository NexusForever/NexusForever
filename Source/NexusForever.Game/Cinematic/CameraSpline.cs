using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Network.Session;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Cinematic
{
    public class CameraSpline : ICameraSpline
    {
        public uint Spline { get; set; }
        public uint SplineMode { get; set; }
        public uint Delay { get; set; }
        public float Speed { get; set; }
        public bool Target { get; set; }
        public bool UseRotation { get; set; }

        public CameraSpline(uint delay, uint spline, uint splineMode, float speed, bool target = false, bool useRotation = true)
        {
            Delay       = delay;
            Spline      = spline;
            SplineMode  = splineMode;
            Speed       = speed;
            Target      = target;
            UseRotation = useRotation;
        }

        public void Send(IGameSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicCameraSpline
            {
                Delay       = Delay,
                Spline      = Spline,
                SplineMode  = SplineMode,
                Speed       = Speed,
                Target      = Target,
                UseRotation = UseRotation
            });
        }
    }
}
