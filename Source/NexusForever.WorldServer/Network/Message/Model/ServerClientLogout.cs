using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{

    [Message(GameMessageOpcode.ServerClientLogout, MessageDirection.Server)]
    public class ServerClientLogout : IWritable
    {
        public bool ClientRequested { get; set; }
        public LogoutReason Reason { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ClientRequested);
            writer.Write(Reason, 5);
        }
    }
}
