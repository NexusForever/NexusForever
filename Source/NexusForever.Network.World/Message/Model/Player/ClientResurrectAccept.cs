using NexusForever.Game.Static.Player;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ClientResurrectAccept)]
    public class ClientResurrectAccept : IReadable
    {
        public uint UnitId { get; private set; }
        public ResurrectionType RezType { get; private set; }
        public uint RezData { get ; private set; }

        public void Read(GamePacketReader reader)
        {
            UnitId = reader.ReadUInt();
            RezType = reader.ReadEnum<ResurrectionType>(7u);
            RezData = reader.ReadUInt();
        }
    }
}
