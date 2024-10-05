using System.Numerics;
using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Game.Abstract.Cinematic.Cinematics;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Cinematic.Cinematics
{
    public class EvilFromTheEtherOnOpenMedbay : CinematicBase, IEvilFromTheEtherOnOpenMedbay
    {
        private const uint ActorCamera      = 75227;
        private const uint ActorMordeshMM01 = 75228;
        private const uint ActorProps       = 75229;
        private const uint ActorMordeshMF01 = 75230;
        private const uint ActorMordeshMF02 = 75231;

        protected override void Setup()
        {
            Duration          = 19800;
            InitialFlags      = 7;
            InitialCancelMode = 2;
            CinematicId       = 0;

            StartTransition = new Transition(0, 1, 2, 1500, 0, 1500);
            EndTransition   = new Transition(18300, 0, 0);

            SetupActors();
            SetupCamera();

            Keyframes.Add("PlayerVisuals",
            [
                new VisualEffect(46873, Player.Guid),
                new VisualEffect(51065, Player.Guid),
            ]);
        }

        private void SetupActors()
        {
            var position = new Position(new Vector3(70.8604f, -850.25f, -121.10999f));

            IActor actor1 = new Actor(ActorCamera, 6, 3.141593f, position);
            AddActor(actor1,
            [
                new VisualEffect(45237)
            ]);

            IActor actor2 = new Actor(ActorMordeshMM01, 6, 3.141593f, position);
            AddActor(actor2,
            [
                new VisualEffect(45237)
            ]);

            IActor actor3 = new Actor(ActorMordeshMF01, 6, 3.141593f, position);
            AddActor(actor3,
            [
                new VisualEffect(45237)
            ]);

            IActor actor4 = new Actor(ActorMordeshMF02, 6, 3.141593f, position);
            AddActor(actor4,
            [
                new VisualEffect(45237)
            ]);

            IActor actor5 = new Actor(ActorProps, 6, 3.141593f, position);
            AddActor(actor5,
            [
                new VisualEffect(45237)
            ]);
        }

        private void SetupCamera()
        {
            IActor cameraActor = GetActor(ActorCamera);

            ICamera camera = new Camera(cameraActor, 7, 0, true, 0);
            camera.AddAttach(6933, 7);
            camera.AddTransition(6933, 0);
            camera.AddAttach(9567, 8);
            camera.AddTransition(9567, 0);
            camera.AddAttach(14300, 9);
            camera.AddTransition(14300, 0);

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
