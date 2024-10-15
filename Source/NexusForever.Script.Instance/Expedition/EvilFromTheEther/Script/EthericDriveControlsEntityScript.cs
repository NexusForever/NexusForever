using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity.Movement.Command.Mode;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Instance.Expedition.EvilFromTheEther.Script
{
    [ScriptFilterCreatureId(71228)]
    public class EthericDriveControlsEntityScript : IOwnedScript<IUnitEntity>
    {
        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public void OnLoad(IUnitEntity owner)
        {
            owner.MovementManager.SetMode(ModeType.Slide);
        }
    }
}
