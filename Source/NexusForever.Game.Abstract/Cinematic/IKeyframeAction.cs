using NexusForever.Network.Session;

namespace NexusForever.Game.Abstract.Cinematic
{
    public interface IKeyframeAction
    {
        void Send(IGameSession session);
    }
}
