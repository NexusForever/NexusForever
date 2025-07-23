using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ClientEntitySelect)]
    public class ClientEntitySelect : IReadable
    {
        public uint UnitId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            UnitId = reader.ReadUInt();
        }
    }
}
