using NexusForever.WorldServer.Game.Cinematic.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Cinematic
{
    public class ActorVisibility : IKeyframeAction
    {
        public uint Delay { get; }
        public Actor Actor { get; }
        public bool Hide { get; }
        public bool Unknown0 { get; }

        public ActorVisibility(uint delay, Actor actor, bool hide = false)
        {
            Delay = delay;
            Actor = actor;
            Hide = hide;
        }

        public void Send(WorldSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicActorVisibility
            {
                Delay = Delay,
                UnitId = Actor.Id,
                Hide = Hide,
                Unknown0 = Unknown0
            });
        }
    }
}
