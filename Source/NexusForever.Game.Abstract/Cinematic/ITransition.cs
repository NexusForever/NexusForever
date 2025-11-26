using NexusForever.Game.Static.Cinematic;

namespace NexusForever.Game.Abstract.Cinematic
{
    public interface ITransition : IKeyframeAction
    {
        uint Delay { get; }
        CameraAddFlags Flags { get; }
        uint EndTransition { get; }
        ushort Start { get; }
        ushort Mid { get; }
        ushort End { get; }
    }
}