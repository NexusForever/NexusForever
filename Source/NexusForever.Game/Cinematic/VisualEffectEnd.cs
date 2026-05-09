using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Network.Session;
using NexusForever.Network.World.Message.Model.Cinematic;

namespace NexusForever.Game.Cinematic
{
    public class VisualEffectEnd : IVisualEffectEnd
    {
        public uint Delay { get; }
        public uint VisualEffectUniqueId { get; }

        public VisualEffectEnd(uint delay, uint visualEffectUniqueId)
        {
            Delay          = delay;
            VisualEffectUniqueId = visualEffectUniqueId;
        }

        public void Send(IGameSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicVisualEffectEnd
            {
                Delay                = Delay,
                VisualEffectUniqueId = VisualEffectUniqueId
            });
        }
    }
}
