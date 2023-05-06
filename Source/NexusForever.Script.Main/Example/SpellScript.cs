using NexusForever.Game.Abstract.Spell;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Example
{
    [ScriptFilterIgnore]
    public class SpellScript : ISpellScript, IOwnedScript<ISpell>
    {
        private ISpell owner;

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public void OnLoad(ISpell owner)
        {
            this.owner = owner;
        }
    }
}
