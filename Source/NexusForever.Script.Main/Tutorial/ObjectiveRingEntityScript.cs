using System.Linq;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Tutorial
{
    /// <summary>
    /// NPE objective rings / holorings: walking or jumping through sets the entity's
    /// <see cref="ISimpleEntity.QuestChecklistIdx"/> bit on matching checklist objectives.
    /// Per-player only — no state is stored on this shared entity script.
    /// </summary>
    [ScriptFilterCreatureId(70939, 73500)]
    public class ObjectiveRingEntityScript : IWorldEntityScript, IOwnedScript<ISimpleEntity>
    {
        // Large enough to catch a jump / hoverboard pass-through; under BaseMap vision (128).
        private const float RingRange = 8f;

        private ISimpleEntity owner;

        #region Dependency Injection

        private readonly IAssetManager assetManager;

        public ObjectiveRingEntityScript(
            IAssetManager assetManager)
        {
            this.assetManager = assetManager;
        }

        #endregion

        public void OnLoad(ISimpleEntity owner)
        {
            this.owner = owner;
        }

        public void OnAddToMap(IBaseMap map)
        {
            owner.SetInRangeCheck(RingRange);
        }

        public void OnEnterRange(IGridEntity entity)
        {
            if (entity is not IPlayer player)
                return;

            foreach (uint targetGroupId in assetManager.GetTargetGroupsForCreatureId(owner.CreatureId) ?? Enumerable.Empty<uint>())
                player.QuestManager.ObjectiveChecklistBit(targetGroupId, owner.QuestChecklistIdx);
        }
    }
}
