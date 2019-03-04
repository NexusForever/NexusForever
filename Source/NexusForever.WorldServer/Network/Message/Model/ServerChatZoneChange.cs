using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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
