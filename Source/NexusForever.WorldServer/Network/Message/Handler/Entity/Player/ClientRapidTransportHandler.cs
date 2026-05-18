using NexusForever.Game.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity.Player
{
    public class ClientRapidTransportHandler : IMessageHandler<IWorldSession, ClientRapidTransport>
    {
        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;

        public ClientRapidTransportHandler(
            IGameTableManager gameTableManager)
        {
            this.gameTableManager = gameTableManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientRapidTransport rapidTransport)
        {
            // TODO: rawaho note, below checks should really happen in the spell effect handler
            //TODO: check for cooldown
            //TODO: handle payment

            TaxiNodeEntry taxiNode = gameTableManager.TaxiNode.GetEntry(rapidTransport.TaxiNode);
            if (taxiNode == null)
                throw new InvalidPacketValueException();

            if (session.Player.Level < taxiNode.AutoUnlockLevel)
                throw new InvalidPacketValueException();

            WorldLocation2Entry worldLocation = gameTableManager.WorldLocation2.GetEntry(taxiNode.WorldLocation2Id);
            if (worldLocation == null)
                throw new InvalidPacketValueException();

            GameFormulaEntry entry = gameTableManager.GameFormula.GetEntry(1307);
            session.Player.CastSpell(entry.Dataint0, new SpellParameters
            {
                TaxiNode = rapidTransport.TaxiNode
            });
        }
    }
}
