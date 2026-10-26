using System;
using System.Collections.Concurrent;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Tutorial
{
    /// <summary>
    /// NPE Extra Booster pads (creature 73461). Retail has no Spell4IdActivate — entering
    /// range casts Power Boost (tiered MountSpeedMultiplier). Per-player only.
    /// </summary>
    [ScriptFilterCreatureId(73461)]
    public class ExtraBoosterEntityScript : IWorldEntityScript, IOwnedScript<ISimpleEntity>
    {
        private const float BoosterRange = 8f;

        // Power Boost - NPEU tiers (1.3x / 1.5x / 1.7x / 2.0x MountSpeedMultiplier, 6s).
        private static readonly uint[] PowerBoostTiers = [84387u, 85479u, 85480u, 85482u];

        private static readonly ConcurrentDictionary<ulong, (byte Tier, DateTime Expires)> playerBoostState = new();

        private ISimpleEntity owner;
        private readonly ConcurrentDictionary<ulong, DateTime> lastHit = new();

        public void OnLoad(ISimpleEntity owner)
        {
            this.owner = owner;
        }

        public void OnAddToMap(IBaseMap map)
        {
            owner.SetInRangeCheck(BoosterRange);
        }

        public void OnEnterRange(IGridEntity entity)
        {
            if (entity is not IPlayer player)
                return;

            DateTime now = DateTime.UtcNow;
            if (lastHit.TryGetValue(player.CharacterId, out DateTime previous) && (now - previous).TotalSeconds < 1d)
                return;
            lastHit[player.CharacterId] = now;

            byte tier = 0;
            if (playerBoostState.TryGetValue(player.CharacterId, out (byte Tier, DateTime Expires) state) && state.Expires > now)
                tier = (byte)Math.Min(state.Tier + 1, PowerBoostTiers.Length - 1);

            // Drop prior boost spell modifiers so tiers replace instead of stacking.
            foreach (uint spellId in PowerBoostTiers)
                player.RemoveSpellProperties(spellId);

            player.CastSpell(PowerBoostTiers[tier]);
            playerBoostState[player.CharacterId] = (tier, now.AddSeconds(8));
        }
    }
}
