using NexusForever.Network.World.Entity;

namespace NexusForever.Game.Abstract.Cinematic
{
    public interface IVisualEffect : IKeyframeAction
    {
        uint Id { get; }
        uint UnitId { get; }
        uint VisualEffectId { get; }
        Position Position { get; }
        uint InitialDelay { get; }
        uint Duration { get; }
        bool RemoveOnCameraEnd { get; }

        void SetActor(IActor unit);
    }
}