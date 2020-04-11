using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NexusForever.WorldServer.Game.Cinematic
{
    public class CinematicBase
    {
        protected Player Player { get; set; }

        public ushort CinematicId { get; set; }
        public uint Duration { get; set; }
        public ushort InitialFlags { get; set; }
        public ushort InitialCancelMode { get; set; }
        public Dictionary</* Id */ uint, Actor> Actors { get; } = new Dictionary<uint, Actor>();
        public Dictionary</* Delay */ uint, /* TextId */ uint> Texts { get; } = new Dictionary<uint, uint>();
        public Dictionary<string, List<IKeyframeAction>> Keyframes { get; } = new Dictionary<string, List<IKeyframeAction>>();
        public List<Camera> Cameras { get; } = new List<Camera>();
        public Transition StartTransition { get; protected set; }
        public Transition EndTransition { get; protected set; }

        protected Actor playerActor;

        public CinematicBase()
        {
        }

        /// <summary>
        /// Add an <see cref="Actor"/>, and any initial <see cref="VisualEffect"/> to the Cinematic playback
        /// </summary>
        protected void AddActor(Actor actor, List<VisualEffect> initialVisuals)
        {
            foreach (VisualEffect visualEffect in initialVisuals)
                actor.AddVisualEffect(visualEffect);

            Actors.Add(actor.Id, actor);
        }

        /// <summary>
        /// Set an <see cref="Actor"/> as the instance the Player will be spawned in. (Important for playback)
        /// </summary>
        protected void SetAsPlayerActor(Actor actor, Position initialPosition, uint unknown3)
        {
            Actor player = new Actor(0, 7, 0f, initialPosition, unknown0: 0);
            player.AddPacketToSend(new ServerCinematic0211
            {
                Unknown0 = 0,
                UnitId = player.Id,
                UnitId1 = actor.Id,
                Unknown3 = unknown3
            });
            playerActor = player;
        }

        /// <summary>
        /// Get a readied <see cref="Actor"/> that is queued for the Cinematic playback based on it's CreatureType ID
        /// </summary>
        protected Actor GetActor(uint creatureType)
        {
            return Actors.Values.FirstOrDefault(i => i.CreatureType == creatureType);
        }

        /// <summary>
        /// Add a Text entry to the Cinematic playback. Will be shown if Cinematic subtitles are enabled by the Player.
        /// </summary>
        protected void AddText(uint textId, uint start, uint end)
        {
            Texts.Add(start, textId);
            Texts.Add(end, 0);
        }

        /// <summary>
        /// Add a <see cref="Camera"/> to the Cinematic playback.
        /// </summary>
        protected void AddCamera(Camera camera)
        {
            Cameras.Add(camera);
        }

        /// <summary>
        /// Starts Playback for this <see cref="CinematicBase"/>, sending the packets to the Player.
        /// </summary>
        public void StartPlayback()
        {
            Player.Session.EnqueueMessageEncrypted(new ServerCinematicNotify
            {
                Flags = InitialFlags,
                Cancel = InitialCancelMode,
                Duration = Duration,
                CinematicId = CinematicId
            });
            if (StartTransition != null)
                StartTransition.Send(Player.Session);

            Play();

            if (EndTransition != null)
                EndTransition.Send(Player.Session);
            Player.Session.EnqueueMessageEncrypted(new ServerCinematicNotify
            {
                Duration = Duration
            });
            Player.Session.EnqueueMessageEncrypted(new ServerCinematicComplete());
        }

        /// <summary>
        /// Sends all packet data to the Player for all <see cref="Actor"/> stored for playback.
        /// </summary>
        protected void SendActors()
        {
            foreach (Actor actor in Actors.Values)
                actor.SendInitialPackets(Player.Session);
        }

        /// <summary>
        /// Sends all packet data to the Player for all Texts stored for playback.
        /// </summary>
        protected void SendTexts()
        {
            foreach ((uint delay, uint textId) in Texts)
                Player.Session.EnqueueMessageEncrypted(new ServerCinematicText
                {
                    Delay = delay,
                    TextId = textId
                });
        }

        /// <summary>
        /// Sends packet data to the Player for the Player <see cref="Actor"/> instance stored for playback.
        /// </summary>
        protected void SendPlayerActor()
        {
            if (playerActor != null)
                playerActor.SendInitialPackets(Player.Session);

            Player.Session.EnqueueMessageEncrypted(new ServerCinematicShowAnimate
            {
                Show = true
            });
            Player.Session.EnqueueMessageEncrypted(new ServerCinematicActorVisibility
            {
                Hide = true
            });

            if (playerActor == null)
                return;

            Player.Session.EnqueueMessageEncrypted(new ServerCinematic022B());
            Player.Session.EnqueueMessageEncrypted(new ServerCinematic0212
            {
                Position = new Position(Player.Position)
            });
        }

        /// <summary>
        /// Sends all packet data to the Player for all <see cref="Camera"/> stored for playback.
        /// </summary>
        protected void SendCameras()
        {
            foreach (Camera camera in Cameras)
                camera.SendInitialPackets(Player.Session);
        }

        /// <summary>
        /// This method calls all individual packet send methods. It may be overridden by a Cinematic Script if playback needs to be customised.
        /// </summary>
        protected virtual void Play()
        {
            SendActors();
            SendPlayerActor();
            SendTexts();

            SendCameras();
        }
    }
}
