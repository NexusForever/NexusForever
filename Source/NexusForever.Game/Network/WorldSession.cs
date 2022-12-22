using System.Net.Sockets;
using NexusForever.Cryptography;
using NexusForever.Database.Auth.Model;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Account;
using NexusForever.Game.Entity;
using NexusForever.Game.RBAC;
using NexusForever.Game.Static;
using NexusForever.Game.Static.RBAC;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.Message.Model;
using NexusForever.Network.Packet;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Network
{
    public class WorldSession : GameSession
    {
        public AccountModel Account { get; private set; }
        public List<CharacterModel> Characters { get; } = new();

        public Player Player { get; set; }

        public AccountRBACManager AccountRbacManager { get; private set; }
        public GenericUnlockManager GenericUnlockManager { get; private set; }
        public AccountCurrencyManager AccountCurrencyManager { get; private set; }
        public EntitlementManager EntitlementManager { get; private set; }

        public AccountTier AccountTier => AccountRbacManager.HasPermission(Permission.Signature) ? AccountTier.Signature : AccountTier.Basic;

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
            Player?.CleanUp();

            // We check that Account isn't null because AuthServer pings World to check if online
            if (Account != null)
                LoginQueueManager.Instance.OnDisconnect(this);
        }

        /// <summary>
        /// Initialise <see cref="WorldSession"/> from an existing <see cref="AccountModel"/> database model.
        /// </summary>
        public void Initialise(AccountModel account)
        {
            if (Account != null)
                throw new InvalidOperationException();

            Account = account;
            NetworkManager<WorldSession>.Instance.UpdateSessionId(this, account.Id.ToString());

            // managers
            AccountRbacManager     = new AccountRBACManager(this, account);
            GenericUnlockManager   = new GenericUnlockManager(this, account);
            AccountCurrencyManager = new AccountCurrencyManager(this, account);
            EntitlementManager     = new EntitlementManager(this, account);
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
