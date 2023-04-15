namespace NexusForever.Game.Abstract.Cinematic
{
    public interface ITransition : IKeyframeAction
    {
        uint Delay { get; }
        uint Flags { get; }
        uint EndTransition { get; }
        ushort Start { get; }
        ushort Mid { get; }
        ushort End { get; }
    }
}