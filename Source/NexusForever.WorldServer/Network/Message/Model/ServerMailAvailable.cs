using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Mail.Network.Message.Model.Shared;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerMailAvailable, MessageDirection.Server)]
    public class ServerMailAvailable : IWritable
    {
        public bool Unknown0 { get; set; } = true;
        public List<MailItem> MailList { get; set; } = new List<MailItem>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);

            writer.Write(MailList.Count);
            MailList.ForEach(v => v.Write(writer));
        }
    }
}
