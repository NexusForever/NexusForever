using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{

    [Message(GameMessageOpcode.ServerClientLogout)]
    public class ServerClientLogout : IWritable
    {
        public bool Requested { get; set; }
        public LogoutReason Reason { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Requested);
            writer.Write(Reason, 5);
        }
    }
}
