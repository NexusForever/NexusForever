namespace NexusForever.Game.Abstract.Cinematic
{
    public interface IVisualEffectEnd : IKeyframeAction
    {
        uint Delay { get; }
        uint VisualEffectId { get; }
    }
}