using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMailResult)]
    public class ServerMailResult : IWritable
    {
        // actions 2 and 3 invoke RefreshMail LUA event
        public uint Action { get; set; }
        public ulong MailId { get; set; }
        public GenericError Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
           writer.Write(Action);
           writer.Write(MailId);
           writer.Write(Result, 8u);
        }
    }
}
