namespace NexusForever.Game.Abstract.Cinematic
{
    public interface IActorVisibility : IKeyframeAction
    {
        uint Delay { get; }
        IActor Actor { get; }
        bool Hide { get; }
        bool Unknown0 { get; }
    }
}