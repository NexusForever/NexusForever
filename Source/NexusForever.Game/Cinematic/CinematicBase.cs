using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Game.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Cinematic
{
    public class CinematicBase : ICinematicBase
    {
        protected Player Player { get; set; }

        public ushort CinematicId { get; set; }
        public uint Duration { get; set; }
        public ushort InitialFlags { get; set; }
        public ushort InitialCancelMode { get; set; }
        public Dictionary</* Id */ uint, IActor> Actors { get; } = new();
        public Dictionary</* Delay */ uint, /* TextId */ uint> Texts { get; } = new();
        public Dictionary<string, List<IKeyframeAction>> Keyframes { get; } = new();
        public List<ICamera> Cameras { get; } = new();
        public ITransition StartTransition { get; protected set; }
        public ITransition EndTransition { get; protected set; }

        protected IActor playerActor;

        public CinematicBase()
        {
        }

        /// <summary>
        /// Add an <see cref="IActor"/>, and any initial <see cref="IVisualEffect"/> to the Cinematic playback
        /// </summary>
        protected void AddActor(IActor actor, List<IVisualEffect> initialVisuals)
        {
            foreach (IVisualEffect visualEffect in initialVisuals)
                actor.AddVisualEffect(visualEffect);

            Actors.Add(actor.Id, actor);
        }

        /// <summary>
        /// Set an <see cref="IActor"/> as the instance the Player will be spawned in. (Important for playback)
        /// </summary>
        protected void SetAsPlayerActor(IActor actor, Position initialPosition, uint unknown3)
        {
            IActor player = new Actor(0, 7, 0f, initialPosition, unknown0: 0);
            player.AddPacketToSend(new ServerCinematic0211
            {
                Unknown0 = 0,
                UnitId   = player.Id,
                UnitId1  = actor.Id,
                Unknown3 = unknown3
            });
            playerActor = player;
        }

        /// <summary>
        /// Get a readied <see cref="IActor"/> that is queued for the Cinematic playback based on it's CreatureType ID
        /// </summary>
        protected IActor GetActor(uint creatureType)
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
        /// Add a <see cref="ICamera"/> to the Cinematic playback.
        /// </summary>
        protected void AddCamera(ICamera camera)
        {
            Cameras.Add(camera);
        }

        /// <summary>
        /// Starts Playback for this <see cref="ICinematicBase"/>, sending the packets to the Player.
        /// </summary>
        public void StartPlayback()
        {
            Player.Session.EnqueueMessageEncrypted(new ServerCinematicNotify
            {
                Flags       = InitialFlags,
                Cancel      = InitialCancelMode,
                Duration    = Duration,
                CinematicId = CinematicId
            });
            StartTransition?.Send(Player.Session);

            Play();

            EndTransition?.Send(Player.Session);
            Player.Session.EnqueueMessageEncrypted(new ServerCinematicNotify
            {
                Duration = Duration
            });
            Player.Session.EnqueueMessageEncrypted(new ServerCinematicComplete());
        }

        /// <summary>
        /// Sends all packet data to the Player for all <see cref="IActor"/> stored for playback.
        /// </summary>
        protected void SendActors()
        {
            foreach (IActor actor in Actors.Values)
                actor.SendInitialPackets(Player.Session);
        }

        /// <summary>
        /// Sends all packet data to the Player for all Texts stored for playback.
        /// </summary>
        protected void SendTexts()
        {
            foreach ((uint delay, uint textId) in Texts)
            {
                Player.Session.EnqueueMessageEncrypted(new ServerCinematicText
                {
                    Delay  = delay,
                    TextId = textId
                });
            }
        }

        /// <summary>
        /// Sends packet data to the Player for the Player <see cref="IActor"/> instance stored for playback.
        /// </summary>
        protected void SendPlayerActor()
        {
            playerActor?.SendInitialPackets(Player.Session);

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
        /// Sends all packet data to the Player for all <see cref="ICamera"/> stored for playback.
        /// </summary>
        protected void SendCameras()
        {
            foreach (ICamera camera in Cameras)
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
