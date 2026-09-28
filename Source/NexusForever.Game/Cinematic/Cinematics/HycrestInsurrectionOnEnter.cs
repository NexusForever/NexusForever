using System.Numerics;
using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Game.Abstract.Cinematic.Cinematics;
using NexusForever.Network.World.Entity;

namespace NexusForever.Game.Cinematic.Cinematics
{
    /// <summary>
    /// Arrival in The Hycrest Insurrection: a short black screen while the players settle on the intro drop ship and it
    /// starts flying; the players then "synchronise" into the simulation (green glow, intro event script).
    /// </summary>
    /// <remarks>
    /// Not built from packet captures. The camera is the GC217 intro camera (actor 70555) at the intro set's origin, playing
    /// its baked timeline (visual 45237); without the animation its bones stay in the rest pose, far below the map. The
    /// camera's fade is held black until just before the cinematic ends, otherwise the fade-in shows the camera's view
    /// for a moment.
    /// </remarks>
    public class HycrestInsurrectionOnEnter : CinematicBase, IHycrestInsurrectionOnEnter
    {
        private const uint ActorCamera = 70555u; // GC217 - Hycrest Adventure Intro - Camera

        private static readonly Vector3 SetOrigin = new(-2520.6f, -873.6975f, -1240f);
        private const float SetAngle = 0f;

        private const uint CinematicTimeline = 45237u; // plays Cinematic_Misc_00 (the whole timeline) on an actor

        private const uint   BlackDuration  = 3000u;
        private const uint   FadeInAt       = BlackDuration - 200u;
        private const ushort BlackHold      = (ushort)FadeInAt;
        private const ushort BlackFadeIn    = 200;
        private const uint   FadeTransition = 2u; // 2 holds black, 3 holds white

        protected override void Setup()
        {
            Duration          = BlackDuration;
            InitialFlags      = 7;
            InitialCancelMode = 0; // can't be skipped
            CinematicId       = 0;

            // a very short start fade gave no black screen at all
            StartTransition = new Transition(0, 1, 2, 1500, 0, 1500);
            EndTransition   = new Transition(FadeInAt, 0, 0);

            var origin = new Position(SetOrigin);
            IActor camera = new Actor(ActorCamera, 6, SetAngle, origin);
            AddActor(camera, [new VisualEffect(CinematicTimeline)]);

            ICamera view = new Camera(camera, 7, 0, true, FadeTransition, 0, BlackHold, BlackFadeIn);
            AddCamera(view);
        }
    }
}
