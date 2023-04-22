using NexusForever.Game.Static.Cinematic;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCinematicState)]
    public class ClientCinematicState : IReadable
    {
        public CinematicState State { get; private set; }
        public bool Unknown0 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            State    = reader.ReadEnum<CinematicState>(8u);
            Unknown0 = reader.ReadBit();
        }
    }
}
