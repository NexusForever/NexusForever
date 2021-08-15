using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientResurrectRequest)]
    public class ClientResurrectRequest : IReadable
    {
        public uint UnitId { get; private set; }
        public RezType RezType { get; private set; }

        public void Read(GamePacketReader reader)
        {
            UnitId = reader.ReadUInt();
            RezType = reader.ReadEnum<RezType>(32u);
        }
    }
}
