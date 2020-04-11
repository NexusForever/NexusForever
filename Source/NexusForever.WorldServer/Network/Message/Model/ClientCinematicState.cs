using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Cinematic.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientCinematicState)]
    public class ClientCinematicState : IReadable
    {
        public CinematicState State { get; private set; }
        public bool Unknown0 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            State = reader.ReadEnum<CinematicState>(8u);
            Unknown0 = reader.ReadBit();
        }
    }
}
