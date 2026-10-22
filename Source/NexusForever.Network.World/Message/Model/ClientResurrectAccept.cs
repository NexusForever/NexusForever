using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientResurrectAccept)]
    public class ClientResurrectAccept : IReadable
    {
        public uint UnitId { get; private set; }
        public byte Unknown1 { get; private set; } // 7 bits between UnitId and RezType; purpose unknown
        public ResurrectionType RezType { get; private set; }

        public void Read(GamePacketReader reader)
        {
            UnitId  = reader.ReadUInt();
            Unknown1 = reader.ReadByte(7u);
            RezType = reader.ReadEnum<ResurrectionType>(32u);
        }
    }
}
