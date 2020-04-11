using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Reputation.Static;
using NexusForever.WorldServer.Network.Message.Model;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NexusForever.WorldServer.Game.Cinematic.Cinematics
{
    class NoviceTutorialOnEnter : CinematicBase
    {
        const uint ACTOR_CAMERA          = 73425;
        const uint ACTOR_CRYOPODS        = 73426;
        const uint ACTOR_NEXUS           = 73428;
        const uint ACTOR_PROPS           = 73430;
        const uint ACTOR_VIRTUAL_DISPLAY = 73431;
        const uint ACTOR_PLAYER          = 73429;
        const uint ACTOR_DORIAN          = 73427;
        const uint ACTOR_DORIAN_HOLO     = 73446;
        const uint ACTOR_ARTEMIS         = 73484;
        const uint ACTOR_ARTEMIS_HOLO    = 73584;
        const uint VFX_ARTEMIS_VOICEOVER = 50912;
        const uint VFX_DORIAN_VOICEOVER  = 50913;

        public NoviceTutorialOnEnter(Player player)
        {
            Player = player;
            Duration = 50000;
            InitialFlags = 7;
            InitialCancelMode = 2;
            StartTransition = new Transition(0, 1, 2, 1500, 0, 1500);
            EndTransition = new Transition(48500, 0, 0);

            Setup();
        }

        public void Setup()
        {
            AddActors();
            SetupCamera();

            if (Player.Faction1 == Faction.Dominion)
                AddDominionTexts();
            else if (Player.Faction1 == Faction.Exile)
                AddExileTexts();

            // Add Scenes
            List<IKeyframeAction> Scenes = new List<IKeyframeAction>
            {
                new Scene(0, 259504),
                new Scene(32500, 262143)
            };
            Keyframes.Add("Scenes", Scenes);

            // Add Screen Effects
            List<IKeyframeAction> ScreenEffects = new List<IKeyframeAction>();
            ScreenEffects.Add(new VisualEffect(50800, Player.Guid, duration: 16100));
            ScreenEffects.Add(new VisualEffect(50694, Player.Guid, initialDelay: 16100, duration: 16400));
            ScreenEffects.Add(new VisualEffect(49483, Player.Guid, initialDelay: 32500, duration: 4000));
            ScreenEffects.Add(new VisualEffect(50700, Player.Guid, initialDelay: 36500));
            Keyframes.Add("ScreenEffects", ScreenEffects);

            // Add Player Effects
            List<IKeyframeAction> PlayerEffects = new List<IKeyframeAction>();
            PlayerEffects.Add(new VisualEffect(21853, Player.Guid));
            // Add Voiceovers
            if (Player.Faction1 == Faction.Dominion)
                PlayerEffects.Add(new VisualEffect(VFX_ARTEMIS_VOICEOVER, Player.Guid));
            else
                PlayerEffects.Add(new VisualEffect(VFX_DORIAN_VOICEOVER, Player.Guid));
            Keyframes.Add("PlayerEffects", PlayerEffects);
        }

        private void SetupCamera()
        {
            Camera mainCam = new Camera(GetActor(ACTOR_CAMERA), 7, 0, true, 0, 1500, 0, 1500);
            mainCam.AddAttach(31000, 8, useRotation: true);
            mainCam.AddTransition(31000, 3, 1500, 0, 1500);
            AddCamera(mainCam);
        }

        private void AddActors()
        {
            Position initialPosition = new Position(new Vector3(41.972198486328125f, -852.9459838867188f, -532.7310180664062f));
            float initialAngle = 3.1415929794311523f;

            AddUniversalActors(initialPosition, initialAngle);
            AddFactionActor(initialPosition, initialAngle);
        }

        private void AddUniversalActors(Position initialPosition, float initialAngle)
        {
            List<uint> actorCreatures = new List<uint>
            {
                ACTOR_CAMERA,
                ACTOR_CRYOPODS,
                ACTOR_NEXUS,
                ACTOR_PROPS,
                ACTOR_VIRTUAL_DISPLAY,
                ACTOR_PLAYER
            };

            foreach (uint actor in actorCreatures)
                AddActor(new Actor(actor, 6, initialAngle, initialPosition), new List<VisualEffect>
                {
                    new VisualEffect(45237)
                });

            uint slot211 = 24;
            switch (Player.Race)
            {
                case Race.Mechari:
                case Race.Mordesh:
                case Race.Granok:
                case Race.Human: // TODO: Find right offset number
                case Race.Draken: // TODO: Find right offset number
                    slot211 = 69;
                    break;
            }
            SetAsPlayerActor(GetActor(ACTOR_PLAYER), initialPosition, slot211);
        }

        private void AddFactionActor(Position initialPosition, float initialAngle)
        {
            uint factionHead = Player.Faction1 == Faction.Dominion ? ACTOR_ARTEMIS : ACTOR_DORIAN;
            uint factionHolo = Player.Faction1 == Faction.Dominion ? ACTOR_ARTEMIS_HOLO : ACTOR_DORIAN_HOLO;

            AddActor(new Actor(factionHead, 6, initialAngle, initialPosition), new List<VisualEffect>
                {
                    new VisualEffect(45237)
                });

            Actor holoActor = new Actor(factionHolo, 6, initialAngle, initialPosition);
            AddActor(holoActor, new List<VisualEffect>
                {
                    new VisualEffect(45237),
                    new VisualEffect(24490)
                });
            holoActor.AddVisibility(9600, true);
            holoActor.AddVisibility(9700, false);
            holoActor.AddVisibility(9800, true);
            holoActor.AddVisibility(9933, false);
            holoActor.AddVisibility(10500, true);
        }

        private void AddDominionTexts()
        {
            AddText(750178, 1700, 5100);
            AddText(750179, 5133, 12033);
            AddText(750180, 12067, 16400);
            AddText(750181, 16433, 20433);
            AddText(750182, 20467, 23733);
            AddText(750183, 23767, 27867);
            AddText(750184, 27900, 34400);
            AddText(750185, 34433, 41067);
            AddText(750186, 41100, 43800);
            AddText(750187, 43833, 49500);
        }

        private void AddExileTexts()
        {
            AddText(750164, 1300, 5767);
            AddText(750165, 5800, 11567);
            AddText(750166, 11600, 16467);
            AddText(750167, 16500, 20733);
            AddText(750168, 20767, 23633);
            AddText(750169, 23667, 28333);
            AddText(750170, 28367, 34333);
            AddText(750171, 34367, 40833);
            AddText(750173, 40867, 44567);
            AddText(750174, 44600, 49500);
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
