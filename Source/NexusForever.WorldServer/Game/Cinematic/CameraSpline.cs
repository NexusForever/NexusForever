using NexusForever.WorldServer.Game.Cinematic.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Cinematic
{
    public class CameraSpline : IKeyframeAction
    {
        public uint Spline { get; set; }
        public uint SplineMode { get; set; }
        public uint Delay { get; set; }
        public float Speed { get; set; }
        public bool Target { get; set; }
        public bool UseRotation { get; set; }

        public CameraSpline(uint delay, uint spline, uint splineMode, float speed, bool target = false, bool useRotation = true)
        {
            Delay = delay;
            Spline = spline;
            SplineMode = splineMode;
            Speed = speed;
            Target = target;
            UseRotation = useRotation;
        }

        public void Send(WorldSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicCameraSpline
            {
                Delay = Delay,
                Spline = Spline,
                SplineMode = SplineMode,
                Speed = Speed,
                Target = Target,
                UseRotation = UseRotation
            });
        }
    }
}
