using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.CSI;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.CSI
{
    public interface IClientSideInteraction
    {
        IWorldEntity ActivateUnit { get; }
        uint ClientUniqueId { get; }
        CSIType CsiType { get; }
        ClientSideInteractionEntry Entry { get; }

        void HandleSuccess(ISpell spellCast);
        void SetClientSideInteractionEntry(ClientSideInteractionEntry clientSideInteractionEntry);
        void TriggerFail();
        void TriggerReady();
        void TriggerSuccess();
    }
}