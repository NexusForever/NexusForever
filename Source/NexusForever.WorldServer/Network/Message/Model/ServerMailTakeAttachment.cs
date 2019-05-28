using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Static;

namespace NexusForever.WorldServer.Network.Message.Model
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
