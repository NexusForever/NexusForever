using System.Net.Sockets;
using NexusForever.AuthServer.Network.Message.Model;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.Shared.Network.Message.Model;

namespace NexusForever.AuthServer.Network
{
    public class AuthSession : GameSession
    {
        public override void OnAccept(Socket newSocket)
        {
            base.OnAccept(newSocket);

            EnqueueMessage(new ServerHello
            {
                AuthVersion    = 16042,
                AuthMessage    = 0x97998A0,
                ConnectionType = 3
            });
        }

        protected override IWritable BuildEncryptedMessage(byte[] data)
        {
            return new ServerAuthEncrypted
            {
                Data = data
            };
        }
    }
}
