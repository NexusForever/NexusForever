using NexusForever.Network.Session;

namespace NexusForever.Game.Abstract.Cinematic
{
    public interface IKeyframeAction
    {
        uint Delay { get; }

        void Send(IGameSession session);
    }
}
