using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Network.Session;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Cinematic
{
    public class VisualEffectEnd : IVisualEffectEnd
    {
        public uint Delay { get; }
        public uint VisualEffectId { get; }

        public VisualEffectEnd(uint delay, uint visualEffectId)
        {
            Delay          = delay;
            VisualEffectId = visualEffectId;
        }

        public void Send(IGameSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicVisualEffectEnd
            {
                Delay        = Delay,
                VisualHandle = VisualEffectId
            });
        }
    }
}
