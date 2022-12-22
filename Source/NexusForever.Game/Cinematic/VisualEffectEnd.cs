using NexusForever.Game.Network;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Cinematic
{
    public class VisualEffectEnd : IKeyframeAction
    {
        public uint Delay { get; }
        public uint VisualEffectId { get; }

        public VisualEffectEnd(uint delay, uint visualEffectId)
        {
            Delay          = delay;
            VisualEffectId = visualEffectId;
        }

        public void Send(WorldSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicVisualEffectEnd
            {
                Delay        = Delay,
                VisualHandle = VisualEffectId
            });
        }
    }
}
