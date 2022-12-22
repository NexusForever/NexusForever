using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{

    [Message(GameMessageOpcode.ServerChatZoneChange)]
    public class ServerChatZoneChange : IWritable
    {
       public ushort WorldZoneId { get; set; }

       public void Write(GamePacketWriter writer)
       {
           writer.Write(WorldZoneId, 15u);
       }
    }
}
