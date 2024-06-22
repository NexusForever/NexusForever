using System;
using System.Collections.Generic;
using System.Net.Sockets;
using NexusForever.Cryptography;
using NexusForever.Database.Auth.Model;
using NexusForever.Database.Character.Model;
using NexusForever.Game;
using NexusForever.Game.Abstract.Account;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Account;
using NexusForever.Game.Static.Entity;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.Message.Model;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network
{
    public class WorldSession : GameSession, IWorldSession
    {
        public IAccount Account { get; private set; }
        public IPlayer Player { get; set; }

        // this really needs to go away
        public List<CharacterModel> Characters { get; } = new();

        /// <summary>
        /// Determines if the <see cref="WorldSession"/> is queued to enter the realm.
        /// </summary>
        /// <remarks>
        /// This occurs when the world has reached the maximum number of allowed players.
        /// </remarks>
        public bool? IsQueued { get; set; }

        public override void OnAccept(Socket newSocket)
        {
            base.OnAccept(newSocket);

            EnqueueMessageEncrypted(new ServerHello
            {
                AuthVersion    = 16042,
                RealmId        = RealmContext.Instance.RealmId,
                RealmGroupId   = 21,
                AuthMessage    = 0x97998A0,
                ConnectionType = 11
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
            Player?.LogoutManager.Finish(LogoutReason.AccountDisconnected);

            // We check that Account isn't null because AuthServer pings World to check if online
            if (Account != null)
                LoginQueueManager.Instance.OnDisconnect(this);
        }

        public override void Update(double lastTick)
        {
            base.Update(lastTick);

            if (Player != null && Player.LogoutManager.State == LogoutState.Finished)
            {
                log.Trace($"Removed player {Player.CharacterId} from session {Id}.");
                Player = null;
            }
        }

        /// <summary>
        /// Returns if <see cref="WorldSession"/> can be disposed.
        /// </summary>
        public override bool CanDispose()
        {
            return base.CanDispose() && Player == null;
        }

        /// <summary>
        /// Initialise <see cref="WorldSession"/> from an existing <see cref="AccountModel"/> database model.
        /// </summary>
        public void Initialise(AccountModel account)
        {
            if (Account != null)
                throw new InvalidOperationException();

            Account = new Account();
            Account.Initialise(account, this);

            NetworkManager<WorldSession>.Instance.UpdateSessionId(this, account.Id.ToString());
        }

        public void SetEncryptionKey(byte[] sessionKey)
        {
            ulong key = PacketCrypt.GetKeyFromTicket(sessionKey);
            encryption = new PacketCrypt(key);

            log.Trace($"Set encryption key {Convert.ToHexString(sessionKey)} for session {Id}.");
        }
    }
}
