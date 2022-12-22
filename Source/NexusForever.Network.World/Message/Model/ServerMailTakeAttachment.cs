using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMailTakeAttachment)]
    public class ServerMailTakeAttachment : IWritable
    {
        public ulong MailId { get; set; }
        public GenericError Result { get; set; }
        public uint Index { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MailId);
            writer.Write(Result, 8u);
            writer.Write(Index);
        }
    }
}
