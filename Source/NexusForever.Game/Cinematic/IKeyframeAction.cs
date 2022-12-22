using NexusForever.Game.Network;

namespace NexusForever.Game.Cinematic
{
    public interface IKeyframeAction
    {
        void Send(WorldSession session);
    }
}
