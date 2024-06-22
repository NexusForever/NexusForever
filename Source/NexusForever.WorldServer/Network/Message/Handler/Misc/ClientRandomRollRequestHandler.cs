using System;
using NexusForever.Game.Abstract;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Misc
{
    public class ClientRandomRollRequestHandler : IMessageHandler<IWorldSession, ClientRandomRollRequest>
    {
        #region Dependency Injection

        private readonly IRealmContext realmContext;

        public ClientRandomRollRequestHandler(
            IRealmContext realmContext)
        {
            this.realmContext = realmContext;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientRandomRollRequest randomRoll)
        {
            if (randomRoll.MinRandom > randomRoll.MaxRandom)
                throw new InvalidPacketValueException();

            if (randomRoll.MaxRandom > 1000000u)
                throw new InvalidPacketValueException();

            session.EnqueueMessageEncrypted(new ServerRandomRollResponse
            {
                TargetPlayerIdentity = new TargetPlayerIdentity
                {
                    RealmId     = realmContext.RealmId,
                    CharacterId = session.Player.CharacterId
                },
                MinRandom        = randomRoll.MinRandom,
                MaxRandom        = randomRoll.MaxRandom,
                RandomRollResult = Random.Shared.Next((int)randomRoll.MinRandom, (int)randomRoll.MaxRandom)
            });
        }
    }
}
