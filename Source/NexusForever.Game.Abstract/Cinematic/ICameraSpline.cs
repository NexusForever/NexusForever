namespace NexusForever.Game.Abstract.Cinematic
{
    public interface ICameraSpline : IKeyframeAction
    {
        uint Spline { get; set; }
        uint SplineMode { get; set; }
        uint Delay { get; set; }
        float Speed { get; set; }
        bool Target { get; set; }
        bool UseRotation { get; set; }
    }
}