using System;
using System.Collections.Generic;
using System.Net.Sockets;
using NexusForever.Shared.Cryptography;
using NexusForever.Shared.Database.Auth.Model;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.Shared.Network.Message.Model;
using NexusForever.Shared.Network.Packet;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network
{
    public class WorldSession : GameSession
    {
        public Account Account { get; private set; }
        public List<Character> Characters { get; } = new List<Character>();

        public Player Player { get; set; }

        public GenericUnlockManager GenericUnlockManager { get; set; }

        public override void OnAccept(Socket newSocket)
        {
            base.OnAccept(newSocket);

            EnqueueMessageEncrypted(new ServerHello
            {
                AuthVersion = 16042,
                RealmId     = WorldServer.RealmId,
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

        protected override void OnDisconnect()
        {
            base.OnDisconnect();
            Player?.CleanUp();
        }

        /// <summary>
        /// Initialise <see cref="WorldSession"/> from an existing <see cref="Account"/> database model.
        /// </summary>
        public void Initialise(Account account)
        {
            if (Account != null)
                throw new InvalidOperationException();

            Account = account;

            GenericUnlockManager = new GenericUnlockManager(this, account);
        }

        public void SetEncryptionKey(byte[] sessionKey)
        {
            ulong key = PacketCrypt.GetKeyFromTicket(sessionKey);
            encryption = new PacketCrypt(key);
        }

        [MessageHandler(GameMessageOpcode.ClientPackedWorld)]
        public void HandlePackedWorld(ClientPackedWorld packedWorld)
        {
            var packet = new ClientGamePacket(packedWorld.Data);
            HandlePacket(packet);
        }
    }
}
