using NexusForever.Game.Static.Cinematic;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    [Message(GameMessageOpcode.ClientCinematicState)]
    public class ClientCinematicState : IReadable
    {
        public CinematicState State { get; private set; }
        public bool NotifyOfCancelledCameraAction { get; private set; } // set by message 0x218

        public void Read(GamePacketReader reader)
        {
            State    = reader.ReadEnum<CinematicState>(8u);
            NotifyOfCancelledCameraAction = reader.ReadBit();
        }
    }
}
