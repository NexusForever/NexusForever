using System.Net.Sockets;
using NexusForever.Network.Auth.Message.Model;
using NexusForever.Network.Message;
using NexusForever.Network.Message.Model;
using NexusForever.Network.Session;

namespace NexusForever.AuthServer.Network
{
    public class AuthSession : GameSession, IAuthSession
    {
        #region Dependency Injection

        public AuthSession(
            IMessageManager messageManager)
            : base(messageManager)
        {
        }

        #endregion

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
