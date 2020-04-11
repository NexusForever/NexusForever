using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Cinematic
{
    public class Actor
    {
        public uint Id { get; }
        public uint InitialDelay { get; }
        public uint CreatureType { get; }
        public ushort Flags { get; }
        public ushort Unknown0 { get; }
        public uint MovementMode { get; }
        public float Angle { get; }
        public bool AngleSet { get; }
        public Position InitialPosition { get; }
        public List<VisualEffect> InitialVisualEffects { get; } = new List<VisualEffect>();
        public List<IKeyframeAction> Keyframes { get; } = new List<IKeyframeAction>();
        public ulong ActivePropId { get; }
        public uint SocketId { get; }

        public List<IWritable> PacketsToSend { get; } = new List<IWritable>();

        public Actor(uint creatureType, ushort flags, float angle, Position position, uint initialDelay = 0, ushort unknown0 = 10, uint movementMode = 3, ulong activePropId = 0, uint socketId = 0)
        {
            Id = GlobalCinematicManager.Instance.NextCinematicId;
            CreatureType = creatureType;
            Flags = flags;
            if (angle != 0f)
            {
                Angle = angle;
                AngleSet = true;
            }
                
            InitialPosition = position;
            InitialDelay = initialDelay;
            Unknown0 = unknown0;
            MovementMode = movementMode;
            ActivePropId = activePropId;
            SocketId = socketId;
        }

        public void AddVisualEffect(VisualEffect visualEffect)
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

        public void SendInitialPackets(WorldSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicActor
            {
                Delay = InitialDelay,
                Flags = Flags,
                Unknown0 = Unknown0,
                SpawnHandle = Id,
                CreatureType = CreatureType,
                MovementMode = MovementMode,
                InitialPosition = InitialPosition,
                ActivePropId = ActivePropId,
                SocketId = SocketId
            });
            
            if (AngleSet)
                session.EnqueueMessageEncrypted(new ServerCinematicActorAngle
                {
                    Delay = 0,
                    UnitId = Id,
                    Angle = Angle
                });

            foreach (VisualEffect visualEffect in InitialVisualEffects)
                visualEffect.Send(session);

            foreach (IWritable message in PacketsToSend)
                session.EnqueueMessageEncrypted(message);
        }
    }
}
