using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Character;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Social
{
    public class ClientPlayerInfoRequestHandler : IMessageHandler<IWorldSession, ClientPlayerInfoRequest>
    {
        #region Dependency Injection

        private readonly ICharacterManager characterManager;
        private readonly IRealmContext realmContext;

        public ClientPlayerInfoRequestHandler(
            ICharacterManager characterManager,
            IRealmContext realmContext)
        {
            this.characterManager = characterManager;
            this.realmContext     = realmContext;
        }

        #endregion

        /// <summary>
        /// Handled responses to Player Info Requests.
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientPlayerInfoRequest request)
        {
            ICharacter character = characterManager.GetCharacter(request.Identity.CharacterId);
            if (character == null)
                throw new InvalidPacketValueException();

            float? onlineStatus = character.GetOnlineStatus();
            session.EnqueueMessageEncrypted(new ServerPlayerInfoFullResponse
            {
                BaseData = new ServerPlayerInfoFullResponse.Base
                {
                    ResultCode = 0,
                    Identity = new TargetPlayerIdentity
                    {
                        RealmId = realmContext.RealmId,
                        CharacterId = character.CharacterId
                    },
                    Name = character.Name,
                    Faction = character.Faction1
                },
                IsClassPathSet = true,
                Path = character.Path,
                Class = character.Class,
                Level = character.Level,
                IsLastLoggedOnInDaysSet = onlineStatus.HasValue,
                LastLoggedInDays = onlineStatus.GetValueOrDefault(0f)
            });
        }
    }
}
