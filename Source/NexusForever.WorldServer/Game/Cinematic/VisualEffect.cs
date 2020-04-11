using NexusForever.WorldServer.Game.Cinematic.Static;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Cinematic
{
    public class VisualEffect : IKeyframeAction
    {
        public uint Id { get; }
        public uint UnitId { get; private set; }
        public uint VisualEffectId { get; }
        public Position Position { get; }
        public uint InitialDelay { get; }
        public uint Duration { get; }
        public bool RemoveOnCameraEnd { get; }

        public VisualEffect(uint visualEffectId, Position position = null, uint initialDelay = 0, bool removeOnCameraEnd = false)
        {
            Id = GlobalCinematicManager.Instance.NextCinematicId;
            VisualEffectId = visualEffectId;

            if (position != null)
                Position = position;
            else
                Position = new Position();

            RemoveOnCameraEnd = removeOnCameraEnd;
            InitialDelay = initialDelay;
        }

        public VisualEffect(uint visualEffectId, uint unitId, Position position = null, uint initialDelay = 0, uint duration = 0, bool removeOnCameraEnd = false)
        {
            Id = GlobalCinematicManager.Instance.NextCinematicId;
            VisualEffectId = visualEffectId;

            if (position != null)
                Position = position;
            else
                Position = new Position();

            UnitId = unitId;
            InitialDelay = initialDelay;
            Duration = duration;
            RemoveOnCameraEnd = removeOnCameraEnd;
        }

        public void SetActor(Actor unit)
        {
            UnitId = unit.Id;
        }

        public void Send(WorldSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicVisualEffect
            {
                Delay = InitialDelay,
                UnitId = UnitId,
                VisualHandle = Id,
                VisualEffectId = VisualEffectId,
                Position = Position,
                RemoveOnCameraEnd = RemoveOnCameraEnd
            });

            if (Duration > 0)
                session.EnqueueMessageEncrypted(new ServerCinematicVisualEffectEnd
                {
                    Delay = InitialDelay + Duration,
                    VisualHandle = Id
                });
        }
    }
}
