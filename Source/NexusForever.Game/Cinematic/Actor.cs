using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Entity.Movement.Command.Mode;
using NexusForever.Network.Message;
using NexusForever.Network.Session;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Cinematic
{
    public class Actor : IActor
    {
        public uint Id { get; }
        public uint InitialDelay { get; }
        public uint Creature2Id { get; }
        public ushort Flags { get; }
        public ushort TextureLevelOfDetailBias { get; }
        public uint MovementMode { get; }
        public float? Angle { get; }
        public Position InitialPosition { get; }
        public List<IVisualEffect> InitialVisualEffects { get; } = new();
        public List<IKeyframeAction> Keyframes { get; } = new();
        public ulong ActivePropId { get; }
        public uint SocketId { get; }

        public List<IWritable> PacketsToSend { get; } = new();

        public Actor(uint creatureType, ushort flags, float? angle, Position position, uint initialDelay = 0, ushort textureLoDBias = 10, uint movementMode = 3, ulong activePropId = 0, uint socketId = 0)
        {
            Id                       = GlobalCinematicManager.Instance.NextCinematicId;
            Creature2Id              = creatureType;
            Flags                    = flags;
            Angle                    = angle;
            InitialPosition          = position;
            InitialDelay             = initialDelay;
            TextureLevelOfDetailBias = textureLoDBias;
            MovementMode             = movementMode;
            ActivePropId             = activePropId;
            SocketId                 = socketId;
        }

        public void AddVisualEffect(IVisualEffect visualEffect)
        {
            visualEffect.SetActor(this);

            InitialVisualEffects.Add(visualEffect);
        }

        public void AddPacketToSend(IWritable packet)
        {
            PacketsToSend.Add(packet);
        }

        public void AddVisibility(uint delay, bool hide)
        {
            Keyframes.Add(new ActorVisibility(delay, this, hide));
        }

        public void SendInitialPackets(IGameSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicActorAdd
            {
                Delay                    = InitialDelay,
                Flags                    = (EntityCreateFlag)Flags,
                TextureLevelOfDetailBias = TextureLevelOfDetailBias,
                UnitId                   = Id,
                Creature2Id              = Creature2Id,
                MovementMode             = (ModeType)MovementMode,
                Position                 = InitialPosition,
                ActivePropId             = ActivePropId,
                WorldSocketId            = SocketId
            });

            if (Angle.HasValue)
            {
                session.EnqueueMessageEncrypted(new ServerCinematicActorAngle
                {
                    Delay  = 0,
                    UnitId = Id,
                    Angle  = Angle.Value
                });
            }

            foreach (IVisualEffect visualEffect in InitialVisualEffects)
                visualEffect.Send(session);

            foreach (IWritable message in PacketsToSend)
                session.EnqueueMessageEncrypted(message);
        }
    }
}
