using NexusForever.Game.Abstract.CSI;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.CSI;
using NexusForever.GameTable.Model;
using NLog;

namespace NexusForever.Game.CSI
{
    /// <summary>
    /// Client Side Interactions (CSI) should be used when the client casts a spell that is not an ability on the LAS.
    /// </summary>
    /// <remarks>
    /// Examples of this are: Using an Item, Interacting with entities in the world, Using Quest-Specific Spells.
    /// Spells triggered in this way are handled slightly differently: 
    /// - ServerSpellStartClientInteraction (0x07FD) Packet sent to the caster.
    /// - ServerSpellStart (0x07FF) Packet sent to nearby players.
    /// </remarks>
    public class ClientSideInteraction : IClientSideInteraction
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public uint ClientUniqueId { get; private set; } = 0;
        public IWorldEntity ActivateUnit { get; private set; } = null;
        public CSIType CsiType { get; private set; } = CSIType.Interaction;
        public ClientSideInteractionEntry Entry { get; private set; }

        private IPlayer Owner { get; set; } = null;

        public ClientSideInteraction(IPlayer owner, IWorldEntity activateUnit, uint clientUniqueId)
        {
            Owner = owner;
            ActivateUnit = activateUnit;
            ClientUniqueId = clientUniqueId;
        }

        public void TriggerReady()
        {
            // This should be used for things like responding after a timer, or ensuring an event happens during an interaction's cast time.
        }

        public void TriggerFail()
        {
            ActivateUnit?.OnActivateFail(Owner);
        }

        public void TriggerSuccess()
        {
            ActivateUnit?.OnActivateSuccess(Owner);
        }

        public void SetClientSideInteractionEntry(ClientSideInteractionEntry clientSideInteractionEntry)
        {
            if (clientSideInteractionEntry == null)
            {
                CsiType = CSIType.Interaction;
                return;
            }

            Entry = clientSideInteractionEntry;
            CsiType = (CSIType)Entry.InteractionType;
        }

        public void HandleSuccess(ISpell spellCast)
        {
            // Placeholder Method in case we need to do things prior to triggering

            TriggerSuccess();
        }
    }
}