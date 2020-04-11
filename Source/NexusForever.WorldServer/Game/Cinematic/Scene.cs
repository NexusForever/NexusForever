using NexusForever.WorldServer.Game.Cinematic.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Cinematic
{
    public class Scene : IKeyframeAction
    {
        public uint Delay { get; }
        public uint SceneId { get; }

        public Scene(uint delay, uint sceneId)
        {
            Delay = delay;
            SceneId = sceneId;
        }

        public void Send(WorldSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicScene
            {
                Delay = Delay,
                SceneId = SceneId
            });
        }
    }
}
