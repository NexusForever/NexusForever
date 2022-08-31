using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Game.Cinematic
{
    public interface IKeyframeAction
    {
        void Send(WorldSession session);
    }
}
