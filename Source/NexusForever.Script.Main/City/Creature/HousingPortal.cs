using NexusForever.Game.Abstract.Entity;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.City.Creature
{
    // 26350: Housing Hologram
    [ScriptFilterCreatureId(26350)]
    public class HousingPortal : ISimpleScript, IOwnedScript<ISimple>
    {
        private uint[] SpellTrainingIds =
        {
            22919, // Recall - House
            25520  // Escape House
        };

        private enum Spells
        {
            ConfirmTeleportDialog = 39111
        }

        private ISimple owner;

        public void Load(ISimple owner)
        {
            this.owner = owner;
        }

        public void OnActivateSuccess(IPlayer activator)
        {
            foreach (uint spellBaseId in SpellTrainingIds)
                if (activator.SpellManager.GetSpell(spellBaseId) == null)
                    activator.SpellManager.AddSpell(spellBaseId);

            activator.CastSpell((uint)Spells.ConfirmTeleportDialog);
        }
    }
}
