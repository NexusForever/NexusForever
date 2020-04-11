using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace NexusForever.WorldServer.Game.Cinematic.Cinematics
{
    public class NoviceTutorialCombatProjector : CinematicBase
    {
        const uint ACTOR_EXILE_CAMERA = 73556;
        const uint ACTOR_EXILE_PLAYER = 73557;
        const uint ACTOR_EXILE_SCANLINES = 74973;
        const uint ACTOR_EXILE_HOUSEPROJ = 74872;
        const uint ACTOR_DOMINION_LEGIONNAIRE = 74891;
        const uint ACTOR_EXILE_ELITE = 74890;
        const uint ACTOR_DOMINION_BATTLEBEAST = 74831;
        const uint ACTOR_EXILE_MINE = 74832;
        const uint ACTOR_EXILE_MINE1 = 74833;
        const uint ACTOR_EXILE_MINE2 = 74834;
        const uint ACTOR_DOMINION_TURRET = 74835;

        public NoviceTutorialCombatProjector(Player player)
        {
            Player = player;
            Duration = 10000;
            InitialFlags = 7;
            InitialCancelMode = 2;
            StartTransition = new Transition(0, 1, 2, 1500, 0, 1500);
            EndTransition = new Transition(8500, 0, 0, 1500, 0, 1500);

            Setup();
        }

        private void Setup()
        {
            SetupActors();
            SetupTexts();
            SetupCamera();

            List<IKeyframeAction> ScreenEffects = new List<IKeyframeAction>();
            ScreenEffects.Add(new VisualEffect(21853, Player.Guid));
            ScreenEffects.Add(new VisualEffect(50915, Player.Guid));
            ScreenEffects.Add(new VisualEffect(50760, Player.Guid, duration: 3000));
            ScreenEffects.Add(new VisualEffect(50761, Player.Guid, initialDelay: 3000));
            Keyframes.Add("ScreenEffects", ScreenEffects);
        }

        private void SetupActors()
        {
            Position initialPosition = new Position(new Vector3(-50.53350067138672f, -861.4010009765625f, 307.8080139160156f));

            AddActor(new Actor(ACTOR_EXILE_CAMERA, 6, 3.1415929794311523f, initialPosition), new List<VisualEffect>
            {
                new VisualEffect(45237)
            });

            Actor player = new Actor(ACTOR_EXILE_PLAYER, 6, 3.1415929794311523f, initialPosition);
            AddActor(player, new List<VisualEffect>
            {
                new VisualEffect(45237)
            });

            AddActor(new Actor(ACTOR_EXILE_SCANLINES, 6, 3.1415929794311523f, initialPosition), new List<VisualEffect>
            {
                new VisualEffect(45237)
            });

            AddActor(new Actor(ACTOR_EXILE_HOUSEPROJ, 6, 0.00001f, new Position(new Vector3(-176.33399963378906f, -847.9630126953125f, 625.9929809570312f))), new List<VisualEffect>
            {
                new VisualEffect(1362)
            });

            AddActor(new Actor(ACTOR_DOMINION_LEGIONNAIRE, 6, -1.2217305898666382f, new Position(new Vector3(-116.96399688720703f, -860.35498046875f, 544.64501953125f))), new List<VisualEffect>
            {
                new VisualEffect(50722),
                new VisualEffect(50723)
            });

            AddActor(new Actor(ACTOR_DOMINION_LEGIONNAIRE, 6, 0.7155850529670715f, new Position(new Vector3(-116.96399688720703f, -860.35498046875f, 544.64501953125f))), new List<VisualEffect>
            {
                new VisualEffect(2224),
                new VisualEffect(2310),
                new VisualEffect(2309)
            });

            AddActor(new Actor(ACTOR_EXILE_ELITE, 6, 1.9024090766906738f, new Position(new Vector3(-112.6760025024414f, -860.2139892578125f, 543.1400146484375f))), new List<VisualEffect>
            {
                new VisualEffect(50720, initialDelay: 2500),
            });

            AddActor(new Actor(ACTOR_EXILE_ELITE, 6, -2.4085545539855957f, new Position(new Vector3(-133.89700317382812f, -859.5999755859375f, 585.364990234375f))), new List<VisualEffect>
            {
                new VisualEffect(50720, initialDelay: 3100),
            });

            AddActor(new Actor(ACTOR_DOMINION_BATTLEBEAST, 6, 0.2617994248867035f, new Position(new Vector3(-143.91900634765625f, -878.0020141601562f, 298.3970031738281f)), initialDelay: 6000), new List<VisualEffect>());
            
            AddActor(new Actor(ACTOR_DOMINION_BATTLEBEAST, 6, 5.7072272300720215f, new Position(new Vector3(-158.04800415039062f, -878.0889892578125f, 310.0090026855469f)), initialDelay: 6000), new List<VisualEffect>());

            AddActor(new Actor(ACTOR_DOMINION_BATTLEBEAST, 6, 1.2566372156143188f, new Position(new Vector3(-169.03700256347656f, -875.8829956054688f, 321.718994140625f)), initialDelay: 6000), new List<VisualEffect>());

            AddActor(new Actor(ACTOR_DOMINION_BATTLEBEAST, 6, 3.263766050338745f, new Position(new Vector3(-135.74000549316406f, -876.4180297851562f, 313.6470031738281f)), initialDelay: 6000), new List<VisualEffect>());

            AddActor(new Actor(ACTOR_DOMINION_BATTLEBEAST, 6, 1.0646508932113647f, new Position(new Vector3(-162.6269989013672f, -875.2310180664062f, 333.4490051269531f)), initialDelay: 6000), new List<VisualEffect>());

            AddActor(new Actor(ACTOR_DOMINION_BATTLEBEAST, 6, 1.29154372215271f, new Position(new Vector3(-106.08899688720703f, -874.8889770507812f, 337.2950134277344f)), initialDelay: 6000), new List<VisualEffect>());

            AddActor(new Actor(ACTOR_DOMINION_BATTLEBEAST, 6, 5.724680423736572f, new Position(new Vector3(-130.56100463867188f, -875.2139892578125f, 341.260009765625f)), initialDelay: 6000), new List<VisualEffect>());
            
            AddActor(new Actor(ACTOR_DOMINION_BATTLEBEAST, 6, 3.5953786373138428f, new Position(new Vector3(-146.36500549316406f, -873.5659790039062f, 351.25201416015625f)), initialDelay: 6000), new List<VisualEffect>());

            AddActor(new Actor(ACTOR_DOMINION_BATTLEBEAST, 6, 4.031711101531982f, new Position(new Vector3(-118.93599700927734f, -872.9459838867188f, 370.3340148925781f)), initialDelay: 6000), new List<VisualEffect>());

            AddActor(new Actor(ACTOR_DOMINION_BATTLEBEAST, 6, 4.031711101531982f, new Position(new Vector3(-118.93599700927734f, -872.9459838867188f, 370.3340148925781f)), initialDelay: 6000), new List<VisualEffect>());

            AddActor(new Actor(ACTOR_EXILE_MINE, 6, 0f, new Position(new Vector3(-70.44529724121094f, -868.8389892578125f, 383.5530090332031f))), new List<VisualEffect>());

            AddActor(new Actor(ACTOR_EXILE_MINE1, 6, 0f, new Position(new Vector3(-87.8593978881836f, -868.8389892578125f, 414.0379943847656f))), new List<VisualEffect>());

            AddActor(new Actor(ACTOR_EXILE_MINE2, 6, 0f, new Position(new Vector3(-119.41000366210938f, -868.8389892578125f, 424.9620056152344f))), new List<VisualEffect>());

            AddActor(new Actor(ACTOR_DOMINION_TURRET, 6, 0.523598849773407f, new Position(new Vector3(-112.69000244140625f, -864.7138671875f, 470.8169860839844f))), new List<VisualEffect>());

            AddActor(new Actor(ACTOR_DOMINION_TURRET, 6, -0.05235988274216652f, new Position(new Vector3(-105.66100311279297f, -865.0139770507812f, 502.4620056152344f))), new List<VisualEffect>());

            SetAsPlayerActor(player, initialPosition, 71);
        }

        private void SetupTexts()
        {
            AddText(749303, 1500, 6400);
            AddText(749304, 6500, 14900);
        }

        private void SetupCamera()
        {
            Camera mainCam = new Camera(GetActor(ACTOR_EXILE_CAMERA), 7, 0, true, 0, 1500, 0, 1500);
            AddCamera(mainCam);
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
