using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Network.Session;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Cinematic
{
    public class Scene : IScene
    {
        public uint Delay { get; }
        public uint Flags { get; }

        public Scene(uint delay, uint flags)
        {
            Delay   = delay;
            Flags = flags;
        }

        public void Send(IGameSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicFlags
            {
                Delay = Delay,
                Flags = Flags
            });
        }
    }
}
