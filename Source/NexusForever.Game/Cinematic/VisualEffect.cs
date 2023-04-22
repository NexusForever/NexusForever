using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Network;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Cinematic
{
    public class VisualEffect : IVisualEffect
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
            Id                = GlobalCinematicManager.Instance.NextCinematicId;
            VisualEffectId    = visualEffectId;

            Position          = position ?? new Position();

            InitialDelay      = initialDelay;
            RemoveOnCameraEnd = removeOnCameraEnd;
        }

        public VisualEffect(uint visualEffectId, uint unitId, Position position = null, uint initialDelay = 0, uint duration = 0, bool removeOnCameraEnd = false)
        {
            Id                = GlobalCinematicManager.Instance.NextCinematicId;
            VisualEffectId    = visualEffectId;

            Position          = position ?? new Position();

            UnitId            = unitId;
            InitialDelay      = initialDelay;
            Duration          = duration;
            RemoveOnCameraEnd = removeOnCameraEnd;
        }

        public void SetActor(IActor unit)
        {
            UnitId = unit.Id;
        }

        public void Send(IGameSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicVisualEffect
            {
                Delay             = InitialDelay,
                UnitId            = UnitId,
                VisualHandle      = Id,
                VisualEffectId    = VisualEffectId,
                Position          = Position,
                RemoveOnCameraEnd = RemoveOnCameraEnd
            });

            if (Duration > 0)
            {
                session.EnqueueMessageEncrypted(new ServerCinematicVisualEffectEnd
                {
                    Delay        = InitialDelay + Duration,
                    VisualHandle = Id
                });
            }
        }
    }
}
