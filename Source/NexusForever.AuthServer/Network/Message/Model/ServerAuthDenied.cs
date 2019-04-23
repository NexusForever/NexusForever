using NexusForever.AuthServer.Network.Message.Static;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.AuthServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerAuthDenied)]
    public class ServerAuthDenied : IWritable
    {
        public NpLoginResult LoginResult { get; set; }
        // if LoginResult > 27 client uses this value in some way for an error code
        public uint Unknown4 { get; set; }
        public float SuspendedDays { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(LoginResult);
            writer.Write(Unknown4);
            writer.Write(SuspendedDays);
        }
    }
}
