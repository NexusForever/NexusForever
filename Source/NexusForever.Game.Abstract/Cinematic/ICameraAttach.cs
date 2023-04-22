namespace NexusForever.Game.Abstract.Cinematic
{
    public interface ICameraAttach : IKeyframeAction
    {
        uint AttachType { get; set; }
        uint AttachId { get; set; }
        uint Delay { get; set; }
        uint ParentUnit { get; set; }
        bool UseRotation { get; set; }
    }
}