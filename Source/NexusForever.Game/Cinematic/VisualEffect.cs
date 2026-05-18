using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Network.Session;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model.Cinematic;

namespace NexusForever.Game.Cinematic
{
    public class VisualEffect : IVisualEffect
    {
        public uint Id { get; }
        public uint UnitId { get; private set; }
        public uint VisualEffectId { get; }
        public Position Position { get; }
        public uint Delay { get; }
        public uint Duration { get; }
        public bool RemoveOnCameraEnd { get; }

        public VisualEffect(uint visualEffectId, Position position = null, uint delay = 0, bool removeOnCameraEnd = false)
        {
            Id                = GlobalCinematicManager.Instance.NextCinematicId;
            VisualEffectId    = visualEffectId;

            Position          = position ?? new Position();

            Delay             = delay;
            RemoveOnCameraEnd = removeOnCameraEnd;
        }

        public VisualEffect(uint visualEffectId, uint unitId, Position position = null, uint delay = 0, uint duration = 0, bool removeOnCameraEnd = false)
        {
            Id                = GlobalCinematicManager.Instance.NextCinematicId;
            VisualEffectId    = visualEffectId;

            Position          = position ?? new Position();

            UnitId            = unitId;
            Delay             = delay;
            Duration          = duration;
            RemoveOnCameraEnd = removeOnCameraEnd;
        }

        public void SetActor(IActor unit)
        {
            UnitId = unit.UnitId;
        }

        public void Send(IGameSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicVisualEffectAdd
            {
                Delay                = Delay,
                UnitId               = UnitId,
                VisualEffectUniqueId = Id,
                VisualEffectId       = VisualEffectId,
                Position             = Position,
                RemoveOnCameraEnd    = RemoveOnCameraEnd
            });

            if (Duration > 0)
            {
                session.EnqueueMessageEncrypted(new ServerCinematicVisualEffectEnd
                {
                    Delay                = Delay + Duration,
                    VisualEffectUniqueId = Id
                });
            }
        }
    }
}
