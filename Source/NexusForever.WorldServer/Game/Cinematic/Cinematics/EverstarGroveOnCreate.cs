using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace NexusForever.WorldServer.Game.Cinematic.Cinematics
{
    public class EverstarGroveOnCreate : CinematicBase
    {
        const uint ACTOR_AURIN_SHIP = 48242;
        const uint ACTOR_RAPTARUK = 35851;

        public EverstarGroveOnCreate(Player player)
        {
            Player = player;
            Duration = 15333;
            InitialFlags = 7;
            InitialCancelMode = 2;
            CinematicId = 35;
            StartTransition = new Transition(0, 1, 2, 1000, 0, 1500);
            EndTransition = new Transition(14333, 0, 0, 1000, 0, 1000);

            Setup();
        }

        private void Setup()
        {
            SetupActors();
            SetupTexts();
            SetupCamera();

            List<IKeyframeAction> ScreenEffects = new List<IKeyframeAction>();
            ScreenEffects.Add(new VisualEffect(21853, Player.Guid));
            ScreenEffects.Add(new VisualEffect(24610, Player.Guid));
            ScreenEffects.Add(new VisualEffect(24612, Player.Guid));
            Keyframes.Add("ScreenEffects", ScreenEffects);
        }

        private void SetupActors()
        {
            Position initialPosition = new Position(new Vector3(-773.25146484375f, -904.217041015625f, -2269.524658203125f));

            Actor ship = new Actor(ACTOR_AURIN_SHIP, 6, 3.1415929794311523f, initialPosition);
            AddActor(ship, new List<VisualEffect>
            {
                new VisualEffect(11096)
            });

            AddActor(new Actor(ACTOR_RAPTARUK, 6, 3.1415929794311523f, initialPosition), new List<VisualEffect>
            {
                new VisualEffect(11096)
            });

            SetAsPlayerActor(ship, initialPosition, 23);
        }

        private void SetupTexts()
        {
            AddText(578488, 4000, 6400);
            AddText(578489, 6500, 10900);
            AddText(578490, 11000, 12600);
            AddText(578491, 12700, 15400);
        }

        private void SetupCamera()
        {
            Camera mainCam = new Camera(10822, 0, 0, 1f, useRotation: true);
            AddCamera(mainCam);
            mainCam.AddTransition(0, 0, 1500, 0, 1500);

            Camera cam2 = new Camera(10823, 4133, 0, 1f, useRotation: true);
            AddCamera(cam2);
            cam2.AddTransition(4133, 0, 1500, 0, 1500);
        }

        protected override void Play()
        {
            base.Play();

            foreach (IKeyframeAction keyframeAction in Keyframes.Values.SelectMany(i => i))
                keyframeAction.Send(Player.Session);
        }
    }
}
