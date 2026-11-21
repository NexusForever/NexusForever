using NexusForever.Game.Static.Option;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Option;

namespace NexusForever.WorldServer.Network.Message.Handler.Option
{
    public class ClientSetInputKeySetHandler : IMessageHandler<IWorldSession, ClientSetInputKeySet>
    {
        public void HandleMessage(IWorldSession session, ClientSetInputKeySet clientSetInputKeySet)
        {
            if (clientSetInputKeySet.InputKeySetEnum is not InputSets.Account and not InputSets.Character)
                throw new InvalidPacketValueException($"Invalid InputKeySet received: {clientSetInputKeySet.InputKeySetEnum}"!);

            session.Player.InputKeySet = clientSetInputKeySet.InputKeySetEnum;
        }
    }
}
