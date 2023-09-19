using NexusForever.Game.Abstract.Cinematic.Cinematics;
using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Network.World.Entity;
using System.Numerics;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Cinematic.Cinematics
{
    public class LevianBayOnCreate : CinematicBase, ILevianBayOnCreate
    {

        protected override void Setup()
        {
            Duration = 34033;
            InitialFlags = 7;
            InitialCancelMode = 2;
            CinematicId = 28;
            StartTransition = new Transition(0, 1, 2, 1000, 0, 1500);
            EndTransition = new Transition(32533, 0, 0);

            SetupActors();
            SetupTexts();
            SetupCamera();

            Keyframes.Add("ScreenEffects", new List<IKeyframeAction>
            {
                new VisualEffect(30667, Player.Guid),
                new VisualEffect(21853, Player.Guid),
                new VisualEffect(29743, Player.Guid),
                new VisualEffect(27968, Player.Guid),
                new VisualEffect(30489, Player.Guid, initialDelay: 4367)
            });

            Keyframes.Add("PlayerVisuals", new List<IKeyframeAction>
            {
                new VisualEffect(0, Player.Guid, removeOnCameraEnd: true),
            });
        }

        private void SetupActors()
        {
            Position initialPosition = new Position(new Vector3(-3784.26953125f, -988.6632690429688f, -6188.072265625f));
            AddActor(new Actor(50444, 14, 3.1415929794311523f, initialPosition), new List<IVisualEffect>
            {
                new VisualEffect(20016),
                new VisualEffect(20016)
            });

            AddActor(new Actor(50441, 14, 3.1415929794311523f, initialPosition), new List<IVisualEffect>
            {
                new VisualEffect(20016)
            });

            AddActor(new Actor(50442, 14, 3.1415929794311523f, initialPosition), new List<IVisualEffect>
            {
                new VisualEffect(20016)
            });

            AddActor(new Actor(0, 7, -1.134464144706726f, new Position(new Vector3(-3858.369384765625f, -973.4382934570312f, -6048.97216796875f))), new List<IVisualEffect>
            {
                new VisualEffect(21598, initialDelay: 26000)
            });
        }

        private void SetupTexts()
        {
            AddText(578464, 4500, 6567);
            AddText(578465, 6633, 10200);
            AddText(578466, 10300, 11400);
            AddText(578467, 11500, 13600);
            AddText(578468, 13700, 18000);
        }

        private void SetupCamera()
        {
            Camera mainCam = new Camera(GetActor(50444), 7, 0, true, 0);
            AddCamera(mainCam);
            mainCam.AddAttach(6333, 8);
            mainCam.AddTransition(6333, 0, 1500, 0, 1500);
            mainCam.AddAttach(23600, 9);
            mainCam.AddTransition(23600, 0, 1500, 0, 1500);
        }

        protected override void Play()
        {
            base.Play();

            Player.Session.EnqueueMessageEncrypted(new ServerCinematicTransitionDurationSet
            {
                Type = 2,
                DurationStart = 1500,
                DurationMid = 0,
                DurationEnd = 1500
            });

            foreach (IKeyframeAction keyframeAction in Keyframes.Values.SelectMany(i => i))
                keyframeAction.Send(Player.Session);
        }
    }
}
