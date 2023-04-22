using NexusForever.Network;

namespace NexusForever.Game.Abstract.Cinematic
{
    public interface ICamera
    {
        IActor CameraActor { get; }
        List<IKeyframeAction> CameraActions { get; }
        
        void AddAttach(uint delay, uint attachId, uint attachType = 0, bool useRotation = true);
        void AddTransition(uint delay, uint type, ushort start = 1500, ushort mid = 0, ushort end = 1500);
        void SendInitialPackets(IGameSession session);
    }
}