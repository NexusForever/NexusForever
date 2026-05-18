using System.Collections.Generic;
using NexusForever.Database.Auth.Model;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Account;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Session;

namespace NexusForever.WorldServer.Network
{
    public interface IWorldSession : IGameSession
    {
        IAccount Account { get; }
        IPlayer Player { get; set; }

        List<CharacterModel> Characters { get; }

        /// <summary>
        /// Determines if the <see cref="WorldSession"/> is queued to enter the realm.
        /// </summary>
        /// <remarks>
        /// This occurs when the world has reached the maximum number of allowed players.
        /// </remarks>
        bool? IsQueued { get; set; }

        /// <summary>
        /// Initialise <see cref="WorldSession"/> from an existing <see cref="AccountModel"/> database model.
        /// </summary>
        void Initialise(AccountModel account);

        void SetEncryptionKey(byte[] sessionKey);
    }
}