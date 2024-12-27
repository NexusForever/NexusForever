using NexusForever.Game.Abstract.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity
{
    public class ClientEntityInteractChairHandler : IMessageHandler<IWorldSession, ClientEntityInteractChair>
    {
        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;

        public ClientEntityInteractChairHandler(
            IGameTableManager gameTableManager)
        {
            this.gameTableManager = gameTableManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientEntityInteractChair entityInteractChair)
        {
            IWorldEntity chair = session.Player.GetVisible<IWorldEntity>(entityInteractChair.ChairUnitId);
            if (chair == null)
                throw new InvalidPacketValueException();

            Creature2Entry creatureEntry = gameTableManager.Creature2.GetEntry(chair.CreatureId);
            if ((creatureEntry.ActivationFlags & 0x200000) == 0)
                throw new InvalidPacketValueException();

            session.Player.Sit(chair);
        }
    }
}
