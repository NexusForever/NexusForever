using NexusForever.Game.Abstract.Combat;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Abstract.Loot.Distribute;

namespace NexusForever.Game.Loot.Distribute
{
    /// <summary>
    /// This loot distributor will roll seperate loot rolls for each individual player and group.
    /// </summary>
    public class IndependentLootDistributor : ILootDistributor
    {
        #region Dependency Injection

        private readonly IGroupManager groupManager;

        public IndependentLootDistributor(
            IGroupManager groupManager)
        {
            this.groupManager = groupManager;
        }

        #endregion

        /// <summary>
        /// Distribute entity loot.
        /// </summary>
        /// <param name="source">Entity to distribute loot from.</param>
        public void Distribute(IUnitEntity source)
        {
            HashSet<IGroup> groups = [];
            HashSet<IPlayer> players = [];

            foreach (IHostileEntity hostile in source.ThreatManager)
            {
                IUnitEntity entity = source.GetVisible<IUnitEntity>(hostile.HatedUnitId);
                if (entity is not IPlayer player)
                    continue;

                IGroup group = groupManager.GetGroup(player.Identity);
                if (group != null)
                    groups.Add(group);
                else
                    players.Add(player);
            }

            foreach (IGroup group in groups)
                source.Map.LootManager.GenerateLoot(group, source);
            foreach (IPlayer player in players)
                source.Map.LootManager.GenerateLoot(player, source);
        }
    }
}
