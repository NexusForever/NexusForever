using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NetworkMessage = NexusForever.Shared.Network.Message.Model.Shared.Message;

namespace NexusForever.AuthServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerRealmMessages)]
    public class ServerRealmMessages : IWritable
    {
        public List<NetworkMessage> Messages { get; set; } = new List<NetworkMessage>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Messages.Count);
            Messages.ForEach(m => m.Write(writer));
        }
    }
}
