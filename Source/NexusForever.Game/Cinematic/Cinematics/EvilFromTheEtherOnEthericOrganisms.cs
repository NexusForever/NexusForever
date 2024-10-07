using System.Numerics;
using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Game.Abstract.Cinematic.Cinematics;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Cinematic.Cinematics
{
    public class EvilFromTheEtherOnEthericOrganisms : CinematicBase, IEvilFromTheEtherOnEthericOrganisms
    {
        private const uint ActorCamera       = 73078;
        private const uint ActorMordesh      = 73079;
        private const uint ActorVoidCreature = 73080;
        private const uint ActorPlayer       = 73081;
        private const uint ActorDatapad      = 73088;

        protected override void Setup()
        {
            Duration          = 18033;
            InitialFlags      = 7;
            InitialCancelMode = 2;
            CinematicId       = 0;

            StartTransition = new Transition(0, 1, 2, 1500, 0, 1500);
            EndTransition   = new Transition(16533, 0, 0);

            SetupActors();
            SetupCamera();

            Keyframes.Add("PlayerVisuals",
            [
                new VisualEffect(21853, Player.Guid),
                new VisualEffect(51562, Player.Guid),
            ]);
        }

        private void SetupActors()
        {
            var position = new Position(new Vector3(-54.04f, -844.93054f, 69.21999f));

            IActor cameraActor = new Actor(ActorCamera, 6, 3.141593f, position);
            AddActor(cameraActor,
            [
                new VisualEffect(45237)
            ]);

            IActor actorMordesh = new Actor(ActorMordesh, 6, 3.141593f, position);
            AddActor(actorMordesh,
            [
                new VisualEffect(45237)
            ]);

            IActor actorVoidCreature = new Actor(ActorVoidCreature, 6, 3.141593f, position);
            AddActor(actorVoidCreature,
            [
                new VisualEffect(45237),
                new VisualEffect(49066),
                new VisualEffect(47542),
                new VisualEffect(47533),
                new VisualEffect(47535),
                new VisualEffect(47541),
                new VisualEffect(47536),
                new VisualEffect(47537),
                new VisualEffect(47538),
                new VisualEffect(47539),
                new VisualEffect(47540),
            ]);

            IActor actorPlayer = new Actor(ActorPlayer, 6, 3.141593f, position);
            AddActor(actorPlayer,
            [
                new VisualEffect(45237)
            ]);

            SetAsPlayerActor(actorPlayer, position, 22);

            IActor actorDatapad = new Actor(ActorDatapad, 6, 3.141593f, position);
            AddActor(actorDatapad,
            [
                new VisualEffect(45237)
            ]);
        }

        private void SetupCamera()
        {
            IActor cameraActor = GetActor(ActorCamera);

            ICamera camera = new Camera(cameraActor, 7, 0, true, 0);
            camera.AddAttach(5000, 8);
            camera.AddTransition(5000, 0);
            camera.AddAttach(7000, 9);
            camera.AddTransition(7000, 0);
            camera.AddAttach(10133, 10);
            camera.AddTransition(10133, 0);
            camera.AddAttach(13667, 11);
            camera.AddTransition(13667, 0);
            camera.AddAttach(15333, 12);
            camera.AddTransition(15333, 0);

            AddCamera(camera);
        }

        protected override void Play()
        {
            base.Play();

            Player.Session.EnqueueMessageEncrypted(new ServerCinematicTransitionDurationSet
            {
                Type          = 2,
                DurationStart = 1500,
                DurationMid   = 0,
                DurationEnd   = 1500
            });

            foreach (IKeyframeAction keyframeAction in Keyframes.Values.SelectMany(i => i))
                keyframeAction.Send(Player.Session);
        }
    }
}
