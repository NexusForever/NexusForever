using NexusForever.Network.Message;
using NetworkMessage = NexusForever.Network.Message.Model.Shared.Message;

namespace NexusForever.Network.Auth.Model
{
    [Message(GameMessageOpcode.ServerRealmMessages)]
    public class ServerRealmMessages : IWritable
    {
        public List<NetworkMessage> Messages { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Messages.Count);
            Messages.ForEach(m => m.Write(writer));
        }
    }
}
