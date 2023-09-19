using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Game.Abstract.Cinematic.Cinematics;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Cinematic.Cinematics
{
    public class CrimsonIsleOnCreate : CinematicBase, ICrimsonIsleOnCreate
    {
        const uint VO_MONDO = 30484;

        protected override void Setup()
        {
            Duration = 18500;
            InitialFlags = 7;
            InitialCancelMode = 2;
            CinematicId = 35;
            StartTransition = new Transition(0, 5, 1, 1500, 0, 1500);
            EndTransition = new Transition(17000, 0, 0);

            SetupActors();
            SetupTexts();
            SetupCamera();

            Keyframes.Add("ScreenEffects", new List<IKeyframeAction>
            {
                new VisualEffect(VO_MONDO, Player.Guid, initialDelay: 950),
            });
        }

        private void SetupActors()
        {
            // TODO: Need parse of Crimson Isle cinematic to finish
        }

        private void SetupTexts()
        {
            AddText(578443, 1000, 2500);
            AddText(578444, 2600, 8000);
            AddText(578445, 8100, 12600);
            AddText(578446, 12700, 16500);
        }

        private void SetupCamera()
        {
            Camera mainCam = new Camera(72364, 0, 0, 1f, useRotation: true);
            AddCamera(mainCam);
            mainCam.AddTransition(0, 0, 1500, 0, 1500);
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
