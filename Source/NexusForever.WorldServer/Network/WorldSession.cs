using System.Collections.Generic;
using System.Net.Sockets;
using NexusForever.Shared.Cryptography;
using NexusForever.Shared.Database.Auth.Model;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.Shared.Network.Message.Model;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network
{
    public class WorldSession : GameSession
    {
        public Account Account { get; set; }
        public List<Character> Characters { get; } = new List<Character>();

        public Player Player { get; set; }

        public override void OnAccept(Socket newSocket)
        {
            base.OnAccept(newSocket);

            EnqueueMessageEncrypted(new ServerHello
            {
                AuthVersion = 16042,
                Unknown4    = 358,
                Unknown8    = 21,
                AuthMessage = 0x97998A0,
                Unknown1C   = 11
            });
        }

        protected override IWritable BuildEncryptedMessage(byte[] data)
        {
            return new ServerRealmEncrypted
            {
                Data = data
            };
        }

        public void SetEncryptionKey(byte[] sessionKey)
        {
            ulong key = PacketCrypt.GetKeyFromTicket(sessionKey);
            encryption = new PacketCrypt(key);
        }
    }
}
