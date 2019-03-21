using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerMailResult, MessageDirection.Server)]
    public class ServerMailResult : IWritable
    {
       public uint Unknown0 { get; set; }
       public ulong Unknown1 { get; set; }
       public byte Unknown2 { get; set; }

       public void Write(GamePacketWriter writer)
       {
           writer.Write(Unknown0);
           writer.Write(Unknown1);
           writer.Write(Unknown2);
       }
    }
}
