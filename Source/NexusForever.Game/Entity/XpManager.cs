using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.GameTable;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Game.Entity
{
    public class XpManager : IXpManager
    {
        private const uint MaximumLevel = 50;

        public uint TotalXp
        {
            get => totalXp;
            private set
            {
                totalXp = value;
                isDirty = true;
            }
        }
        private uint totalXp;

        public uint RestBonusXp
        {
            get => restBonusXp;
            private set
            {
                restBonusXp = value;
                isDirty = true;
            }
        }
        private uint restBonusXp;

        private bool isDirty;
        private readonly IPlayer player;

        /// <summary>
        /// Create a new <see cref="IXpManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public XpManager(IPlayer player, CharacterModel model)
        {
            this.player = player;
            totalXp = model.TotalXp;
            restBonusXp = model.RestBonusXp;

            CalculateRestXpAtLogin(model);
        }

        public void Save(CharacterContext context)
        {
            if (!isDirty)
                return;

            // character is attached in Player::Save, this will only be local lookup
            CharacterModel character = context.Character.Find(player.CharacterId);
            character.TotalXp = TotalXp;
            character.RestBonusXp = RestBonusXp;

            EntityEntry<CharacterModel> entity = context.Entry(character);
            entity.Property(p => p.TotalXp).IsModified = true;
            entity.Property(p => p.RestBonusXp).IsModified = true;

            isDirty = false;
        }

        private void CalculateRestXpAtLogin(CharacterModel model)
        {
            if (player.Level >= MaximumLevel)
            {
                if (RestBonusXp != 0)
                    RestBonusXp = 0;
                return;
            }

            if (model.LastOnline is not DateTime lastOnline)
                return;

            var levels = GameTableManager.Instance.XpPerLevel;
            uint xpForLevel = levels.GetEntry(player.Level).MinXpForLevel;
            uint xpForNextLevel = levels.GetEntry(player.Level + 1).MinXpForLevel;
            uint levelSpan = xpForNextLevel - xpForLevel;
            uint maximumBonusXp = levelSpan + levelSpan / 2;

            // Rest is currently accrued in housing. Other locations keep the already earned reserve.
            double offlineHours = Math.Max(0d, (DateTime.UtcNow - lastOnline).TotalHours);
            double earnedRest = model.WorldId == 1229 ? levelSpan * offlineHours * 0.0024d : 0d;
            uint nextRest = (uint)Math.Min(maximumBonusXp, (ulong)RestBonusXp + (uint)Math.Min(earnedRest, maximumBonusXp));
            if (nextRest != RestBonusXp)
                RestBonusXp = nextRest;
        }

        /// <summary>
        /// Grants <see cref="IPlayer"/> the supplied experience, handling level up if necessary.
        /// </summary>
        /// <param name="earnedXp">Experience to grant</param>
        /// <param name="reason"><see cref="ExpReason"/> for the experience grant</param>
        public void GrantXp(uint earnedXp, ExpReason reason = ExpReason.Cheat)
        {
            if (earnedXp == 0 || player.Level >= MaximumLevel)
                return;

            var levels = GameTableManager.Instance.XpPerLevel;
            uint cap = levels.GetEntry(MaximumLevel).MinXpForLevel;
            uint available = TotalXp < cap ? cap - TotalXp : 0;
            uint appliedXp = Math.Min(earnedXp, available);
            uint signatureXp = player.SignatureEnabled
                ? Math.Min(appliedXp / 4, available - appliedXp) : 0u;
            uint restXp = reason == ExpReason.KillCreature
                ? Math.Min(RestBonusXp, Math.Min(appliedXp / 2, available - appliedXp - signatureXp)) : 0u;
            uint gained = appliedXp + signatureXp + restXp;

            if (gained != 0)
            {
                player.Session.EnqueueMessageEncrypted(new ServerExperienceGained
                {
                    TotalXpGained     = gained,
                    RestXpAmount      = restXp,
                    SignatureXpAmount = signatureXp,
                    Reason            = reason
                });
            }

            uint updatedTotal = TotalXp + gained;
            while (player.Level < MaximumLevel && updatedTotal >= levels.GetEntry(player.Level + 1).MinXpForLevel)
                GrantLevel((byte)(player.Level + 1));

            if (gained != 0)
                TotalXp = updatedTotal;

            uint remainingRest = player.Level == MaximumLevel ? 0 : RestBonusXp - restXp;
            if (remainingRest != RestBonusXp)
                RestBonusXp = remainingRest;
        }

        /// <summary>
        /// Sets <see cref="IPlayer"/> to the supplied level and adjusts XP accordingly. Mainly for use with GM commands.
        /// </summary>
        /// <param name="newLevel">New level to be set</param>
        /// <param name="reason"><see cref="ExpReason"/> for the level grant</param>
        public void SetLevel(byte newLevel, ExpReason reason = ExpReason.Cheat)
        {
            if (newLevel == player.Level)
                return;

            uint newXp = GameTableManager.Instance.XpPerLevel.GetEntry(newLevel).MinXpForLevel;
            player.Session.EnqueueMessageEncrypted(new ServerExperienceGained
            {
                TotalXpGained     = newXp - TotalXp,
                RestXpAmount      = 0,
                SignatureXpAmount = 0,
                Reason            = reason
            });

            TotalXp = newXp;
            GrantLevel(newLevel);
        }

        /// <summary>
        /// Grants <see cref="IPlayer"/> the supplied level and adjusts XP accordingly
        /// </summary>
        /// <param name="newLevel">New level to be set</param>
        private void GrantLevel(byte newLevel)
        {
            uint oldLevel = player.Level;
            if (newLevel == oldLevel)
                return;

            player.Level = newLevel;

            // Grant Rewards for level up
            player.SpellManager.GrantSpells();
            // Unlock LAS slots
            // Unlock AMPs
            // Add feature access
        }
    }
}
