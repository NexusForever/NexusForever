using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Shared.GameTable;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;
using System;

namespace NexusForever.WorldServer.Game.Entity
{
    public class XpManager
    {
        public uint TotalXp
        {
            get => totalXp;
            private set
            {
                totalXp = value;
                saveMask |= PlayerSaveMask.Xp;
            }
        }
        private uint totalXp;

        public uint RestBonusXp
        {
            get => restBonusXp;
            private set
            {
                restBonusXp = value;
                saveMask |= PlayerSaveMask.Xp;
            }
        }
        private uint restBonusXp;

        private PlayerSaveMask saveMask;
        private readonly Player player;

        /// <summary>
        /// Create a new <see cref="XpManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public XpManager(Player player, CharacterModel model)
        {
            this.player = player;
            totalXp = model.TotalXp;

            CalculateRestXpAtLogin(model);
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == PlayerSaveMask.None)
                return;

            if ((saveMask & PlayerSaveMask.Xp) != 0)
            {
                // character is attached in Player::Save, this will only be local lookup
                CharacterModel character = context.Character.Find(player.CharacterId);
                character.TotalXp = TotalXp;
                character.RestBonusXp = RestBonusXp;

                EntityEntry<CharacterModel> entity = context.Entry(character);
                entity.Property(p => p.TotalXp).IsModified = true;
                entity.Property(p => p.RestBonusXp).IsModified = true;
            }

            saveMask = PlayerSaveMask.None;
        }

        private void CalculateRestXpAtLogin(CharacterModel model)
        {
            // don't calculate rest xp for first login
            if (model.LastOnline == null)
                return;

            float xpForLevel     = GameTableManager.Instance.XpPerLevel.GetEntry(player.Level).MinXpForLevel;
            float xpForNextLevel = GameTableManager.Instance.XpPerLevel.GetEntry(player.Level + 1).MinXpForLevel;

            uint maximumBonusXp;
            if (player.Level < 50)
                maximumBonusXp = (uint)((xpForNextLevel - xpForLevel) * 1.5f);
            else
                maximumBonusXp = 0; // TODO: Calculate Elder Gem Rest Bonus XP

            double xpPercentEarned;

            // TODO: Calculate Rest Bonus XP earned since last login, properly.
            // Data from this video was used in initial calculations: https://www.youtube.com/watch?v=xEMMd7CGg4s
            // Video is out of date, but the assumption is the formulas are the same just modified more post-F2P.
            double hoursSinceLogin = DateTime.UtcNow.Subtract((DateTime)model.LastOnline).TotalHours;
            switch (model.WorldId)
            {
                case 1229:
                    // TODO: Apply bonuses from decor or other things that increase rested XP gain.
                    xpPercentEarned = hoursSinceLogin * 0.0024f;
                    break;
                // TODO: Add support for home cities, towns and sleeping bag (?!) gain rates.
                default:
                    xpPercentEarned = 0d;
                    break;
            }

            // TODO: Apply bonuses from spells as necessary

            uint bonusXpValue = Math.Clamp((uint)((xpForNextLevel - xpForLevel) * xpPercentEarned), 0, maximumBonusXp);
            uint totalBonusXp = Math.Clamp(model.RestBonusXp + bonusXpValue, 0u, maximumBonusXp);
            RestBonusXp = totalBonusXp;
        }

        /// <summary>
        /// Grants <see cref="Player"/> the supplied experience, handling level up if necessary.
        /// </summary>
        /// <param name="earnedXp">Experience to grant</param>
        /// <param name="reason"><see cref="ExpReason"/> for the experience grant</param>
        public void GrantXp(uint earnedXp, ExpReason reason = ExpReason.Cheat)
        {
            // TODO: move to configuration option
            const uint maxLevel = 50;

            if (earnedXp < 1)
                return;

            //if (!IsAlive)
            //    return;

            if (player.Level >= maxLevel)
                return;

            // TODO: Apply XP bonuses from current spells or active events

            // Signature XP rate was 25% extra. 
            uint signatureXp = 0u;
            if (player.SignatureEnabled)
                signatureXp = (uint)(earnedXp * 0.25f); // TODO: Make rate configurable.

            // Calculate Rest XP Bonus
            uint restXp = 0u;
            if (reason == ExpReason.KillCreature)
                restXp = (uint)(earnedXp * 0.5f);

            player.Session.EnqueueMessageEncrypted(new ServerExperienceGained
            {
                TotalXpGained     = earnedXp + signatureXp + restXp,
                RestXpAmount      = restXp,
                SignatureXpAmount = signatureXp,
                Reason            = reason
            });

            uint totalXp = TotalXp + earnedXp + signatureXp + restXp;

            uint xpToNextLevel = GameTableManager.Instance.XpPerLevel.GetEntry(player.Level + 1).MinXpForLevel;
            while (totalXp >= xpToNextLevel && player.Level < maxLevel) // WorldServer.Rules.MaxLevel)
            {
                totalXp -= xpToNextLevel;
                GrantLevel((byte)(player.Level + 1));

                xpToNextLevel = GameTableManager.Instance.XpPerLevel.GetEntry(player.Level + 1).MinXpForLevel;
            }

            TotalXp += earnedXp + signatureXp + restXp;
        }

        /// <summary>
        /// Sets <see cref="Player"/> to the supplied level and adjusts XP accordingly. Mainly for use with GM commands.
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
        /// Grants <see cref="Player"/> the supplied level and adjusts XP accordingly
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
