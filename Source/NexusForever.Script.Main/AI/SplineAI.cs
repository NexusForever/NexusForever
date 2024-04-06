using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Entity.Movement.Command.Mode;
using NexusForever.Game.Static.Entity.Movement.Spline;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;
using NexusForever.Script.Template.Filter.Dynamic;

namespace NexusForever.Script.Main.AI
{
    [ScriptFilterDynamic<IScriptFilterDynamicEntitySpline>]
    //[ScriptFilterIgnore]
    public class SplineAI : IWorldEntityScript, IOwnedScript<ICreatureEntity>
    {
        private ICreatureEntity owner;

        public void OnLoad(ICreatureEntity owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to <see cref="IBaseMap"/>.
        /// </summary>
        public void OnAddToMap(IBaseMap map)
        {
            if (owner.Spline == null)
                return;

            // TODO: Rawaho, handle negative spline speed and additional modes...
            if (owner.Spline.Mode > SplineMode.CyclicReverse || owner.Spline.Speed == -1)
                return;

            owner.MovementManager.SetMode(ModeType.Walk);
            owner.MovementManager.LaunchSpline(owner.Spline.SplineId, owner.Spline.Mode, owner.Spline.Speed, false);
        }
    }
}
