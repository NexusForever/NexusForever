namespace NexusForever.Game.Abstract.Cinematic
{
    public interface IScene : IKeyframeAction
    {
        uint Delay { get; }
        uint Flags { get; }
    }
}