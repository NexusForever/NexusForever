using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ClientItemSalvage)]
    public class ClientItemSalvage : IReadable
    {
        // Not sure why message sends both of these. The server should be able to determine the item location from the ItemGuid
        // or get the ItemGuid from the ItemLocation and character's Identity.
        public ItemLocation From { get; private set; } = new ItemLocation();
        public ulong ItemGuid { get; private set; }

        public void Read(GamePacketReader reader)
        {
            From.Read(reader);
            ItemGuid = reader.ReadULong();
        }
    }
}
