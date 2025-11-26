using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Network.Session;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Cinematic
{
    public class ActorVisibility : IActorVisibility
    {
        public uint Delay { get; }
        public IActor Actor { get; }
        public bool Hide { get; }
        public bool AffectOnlyPlayers { get; }

        public ActorVisibility(uint delay, IActor actor, bool hide = false)
        {
            Delay = delay;
            Actor = actor;
            Hide  = hide;
        }

        public void Send(IGameSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicActorVisibility
            {
                Delay             = Delay,
                UnitId            = Actor.UnitId,
                Hide              = Hide,
                AffectOnlyPlayers = AffectOnlyPlayers
            });
        }
    }
}
