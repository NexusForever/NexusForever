using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Network.Session;

namespace NexusForever.Game.Cinematic
{
    public class Camera : ICamera
    {
        public IActor CameraActor { get; }
        public List<IKeyframeAction> CameraActions { get; } = new();

        public Camera(IActor cameraActor, uint initialAttachId, uint initialAttachType, bool useRotationInitially, uint initialTransitionType, ushort initialTransitionStart = 1500, ushort initialTransitionMid = 0, ushort initialTransitionEnd = 1500)
        {
            CameraActor = cameraActor;

            AddAttach(0, initialAttachId, initialAttachType, useRotationInitially);
            AddTransition(0, initialTransitionType, initialTransitionStart, initialTransitionMid, initialTransitionEnd);
        }

        public Camera(uint spline, uint splineMode, uint delay, float speed, bool target = false, bool useRotation = true)
        {
            CameraSpline cameraSpline = new CameraSpline(delay, spline, splineMode, speed, target, useRotation);
            CameraActions.Add(cameraSpline);
        }

        public void AddAttach(uint delay, uint attachId, uint attachType = 0, bool useRotation = true)
        {
            CameraAttach cameraAttach = new CameraAttach(delay, attachId, this, attachType, useRotation);
            CameraActions.Add(cameraAttach);
        }

        public void AddTransition(uint delay, uint type, ushort start = 1500, ushort mid = 0, ushort end = 1500)
        {
            CameraTransition cameraTransition = new CameraTransition(delay, type, start, mid, end);
            CameraActions.Add(cameraTransition);
        }

        public void SendInitialPackets(IGameSession session)
        {
            foreach (IKeyframeAction action in CameraActions)
                action.Send(session);
        }
    }
}
