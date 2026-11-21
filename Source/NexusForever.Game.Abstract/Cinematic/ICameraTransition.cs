namespace NexusForever.Game.Abstract.Cinematic
{
    public interface ICameraTransition : IKeyframeAction
    {
        uint Type { get; set; }
        ushort DurationStart { get; set; }
        ushort DurationMid { get; set; }
        ushort DurationEnd { get; set; }
    }
}