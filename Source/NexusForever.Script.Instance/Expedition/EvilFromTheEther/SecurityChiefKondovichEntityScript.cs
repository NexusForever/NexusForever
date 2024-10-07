using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Event;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Instance.Expedition.EvilFromTheEther
{
    [ScriptFilterCreatureId(71287)]
    public class SecurityChiefKondovichEntityScript : INonPlayerScript, IOwnedScript<INonPlayerEntity>
    {
        private INonPlayerEntity kondovich;

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public void OnLoad(INonPlayerEntity owner)
        {
            kondovich = owner;
        }

        /// <summary>
        /// Invoked when <see cref="IUnitEntity"/> is killed.
        /// </summary>
        public void OnDeath()
        {
            kondovich.Map.PublicEventManager.UpdateObjective(PublicEventObjectiveType.KillEventObjectiveUnit, 0, 1);
        }
    }
}
