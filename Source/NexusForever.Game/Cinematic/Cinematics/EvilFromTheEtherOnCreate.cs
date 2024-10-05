using System.Numerics;
using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Game.Abstract.Cinematic.Cinematics;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Cinematic.Cinematics
{
    public class EvilFromTheEtherOnCreate : CinematicBase, IEvilFromTheEtherOnCreate
    {
        private const uint ActorCamera       = 71591;
        private const uint ActorLogo         = 71592;
        private const uint ActorPlayer       = 71593;
        private const uint ActorShip         = 71594;
        private const uint ActorSpace        = 71595;
        private const uint ActorSpaceStation = 71596;

        protected override void Setup()
        {
            Duration          = 28200;
            InitialFlags      = 7;
            InitialCancelMode = 2;
            CinematicId       = 0;

            StartTransition = new Transition(0, 1, 2, 1500, 0, 1500);
            EndTransition   = new Transition(26700, 0, 0);

            SetupActors();
            SetupTexts();
            SetupCamera();

            Keyframes.Add("PlayerVisuals",
            [
                new VisualEffect(46873, Player.Guid, null, 0, 20000),
                new VisualEffect(47937, Player.Guid, null, 20000, 2500),
                new VisualEffect(47936, Player.Guid, null, 22500),
                new VisualEffect(21853, Player.Guid),
                new VisualEffect(51030, Player.Guid),
            ]);
        }

        private void SetupActors()
        {
            var position = new Position(new Vector3(-433.70453f, -844.3665f, 118.8544f));

            IActor cameraActor = new Actor(ActorCamera, 6, 3.141593f, position);
            AddActor(cameraActor,
            [ 
                new VisualEffect(45237)
            ]);

            IActor shipActor = new Actor(ActorShip, 6, 3.141593f, position);
            AddActor(shipActor,
            [ 
                new VisualEffect(45237)
            ]);

            IActor logoActor = new Actor(ActorLogo, 6, 3.141593f, position);
            AddActor(logoActor,
            [ 
                new VisualEffect(45237)
            ]);

            IActor spaceActor = new Actor(ActorSpace, 6, 3.141593f, position);
            AddActor(spaceActor,
            [ 
                new VisualEffect(45237)
            ]);

            IActor spaceStationActor = new Actor(ActorSpaceStation, 6, 3.141593f, position);
            AddActor(spaceStationActor,
            [ 
                new VisualEffect(45237)
            ]);

            IActor playerActor = new Actor(ActorPlayer, 6, 3.141593f, position);
            AddActor(playerActor,
            [ 
                new VisualEffect(45237)
            ]);

            SetAsPlayerActor(playerActor, position, 22);
        }

        private void SetupTexts()
        {
            AddText(759749, 1000, 9400);
            AddText(759750, 9500, 18000);
            AddText(759751, 18100, 24200);
        }

        private void SetupCamera()
        {
            IActor cameraActor = GetActor(ActorCamera);

            ICamera camera = new Camera(cameraActor, 7, 0, true, 0);
            camera.AddAttach(9067, 8);
            camera.AddTransition(9067, 0);
            camera.AddAttach(14100, 9);
            camera.AddTransition(14100, 0);
            camera.AddAttach(21667, 10);
            camera.AddTransition(21667, 0);

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
