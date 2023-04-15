using NexusForever.Network;

namespace NexusForever.Game.Abstract.Cinematic
{
    public interface IKeyframeAction
    {
        void Send(IGameSession session);
    }
}
