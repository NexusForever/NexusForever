﻿using System.Diagnostics;
using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Achievement;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Prerequisite;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Achievement;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Achievement
{
    public abstract class BaseAchievementManager<T> : IBaseAchievementManager<T> where T : class, IAchievementModel, new()
    {
        public uint AchievementPoints { get; protected set; }

        protected abstract ulong OwnerId { get; }
        protected Dictionary<ushort, IAchievement> achievements = new();

        /// <summary>
        /// Initialise a collection of existing achievement database models.
        /// </summary>
        public void Initialise(IEnumerable<T> models, bool isPlayer)
        {
            foreach (T model in models)
            {
                IAchievementInfo info = GlobalAchievementManager.Instance.GetAchievement(model.AchievementId);
                if (info == null)
                    throw new DatabaseDataException($"{(isPlayer ? "Player" : "Guild")} {model.Id} has invalid achievement {model.AchievementId} stored!");

                if (isPlayer && !info.IsPlayerAchievement)
                    throw new DatabaseDataException($"Player {model.Id} has guild achievement {model.AchievementId} stored!");
                if (!isPlayer && info.IsPlayerAchievement)
                    throw new DatabaseDataException($"Guild {model.Id} has player achievement {model.AchievementId} stored!");

                var achievement = new Achievement<T>(info, model);
                if (achievement.IsComplete())
                    AchievementPoints += GetAchievementPoints(achievement.Info);

                achievements.Add(achievement.Id, achievement);
            }
        }

        public void Save(CharacterContext context)
        {
            foreach (Achievement<T> achievement in achievements.Values)
                achievement.Save(context);
        }

        /// <summary>
        /// Returns if the supplied achievement id has been completed.
        /// </summary>
        public bool HasCompletedAchievement(ushort id)
        {
            return achievements.TryGetValue(id, out IAchievement achievement) && achievement.IsComplete();
        }

        /// <summary>
        /// Send initial <see cref="IAchievement"/> information to owner on login.
        /// </summary>
        public virtual void SendInitialPackets(IPlayer target)
        {
            target.Session.EnqueueMessageEncrypted(new ServerAchievementInit
            {
                Achievements = achievements.Values
                    .Select(a => a.Build())
                    .ToList()
            });
        }

        protected ServerAchievementUpdate BuildAchievementUpdate(IEnumerable<IAchievement> updates)
        {
            return new()
            {
                Achievements = updates
                    .Select(a => a.Build())
                    .ToList()
            };
        }

        protected void SendAchievementUpdate(params IAchievement[] updates)
        {
            SendAchievementUpdate(updates.AsEnumerable());
        }

        protected abstract void SendAchievementUpdate(IEnumerable<IAchievement> updates);

        /// <summary>
        /// Grant achievement by supplied achievement id.
        /// </summary>
        public void GrantAchievement(ushort id)
        {
            IAchievementInfo info = GlobalAchievementManager.Instance.GetAchievement(id);
            if (info == null)
                throw new ArgumentException();

            if (HasCompletedAchievement(info.Id))
                throw new ArgumentException();

            IAchievement achievement = GetAchievement(id);
            if (info.ChecklistEntries.Count == 0)
                achievement.Data0 = info.Entry.Value;
            else
                foreach (AchievementChecklistEntry entry in info.ChecklistEntries)
                    achievement.Data0 |= 1u << (int)entry.Bit;

            Debug.Assert(achievement.IsComplete());
            CompleteAchievement(achievement);
            SendAchievementUpdate(achievement);
        }

        /// <summary>
        /// Update or complete any achievements of <see cref="AchievementType"/> as <see cref="IPlayer"/> with supplied object ids.
        /// </summary>
        public abstract void CheckAchievements(IPlayer target, AchievementType type, uint objectId, uint objectIdAlt = 0u, uint count = 1u);

        /// <summary>
        /// Update or complete a collection of achievements as <see cref="IPlayer"/> sending the result to the client.
        /// </summary>
        protected void CheckAchievements(IPlayer target, IEnumerable<IAchievementInfo> achievements, uint objectId, uint objectIdAlt, uint count)
        {
            var updates = new List<IAchievement>();
            foreach (IAchievementInfo info in achievements)
                if (CheckAchievement(target, info, objectId, objectIdAlt, count))
                    updates.Add(GetAchievement(info.Id));

            if (updates.Count != 0)
                SendAchievementUpdate(updates);
        }

        /// <summary>
        /// Update or complete <see cref="AchievementInfo"/> as <see cref="IPlayer"/> with supplied object ids.
        /// </summary>
        private bool CheckAchievement(IPlayer target, IAchievementInfo info, uint objectId, uint objectIdAlt, uint count)
        {
            if (HasCompletedAchievement(info.Id))
                return false;

            if (DisableManager.Instance.IsDisabled(DisableType.Achievement, info.Id))
                return false;

            bool sendUpdate = false;

            IAchievement achievement = null;
            if (info.ChecklistEntries.Count == 0)
            {
                if (CanUpdateAchievement(target, info.Entry, objectId, objectIdAlt))
                {
                    achievement = GetAchievement(info.Id);
                    achievement.Data0 += count;
                    sendUpdate = true;
                }
            }
            else
            {
                achievement = GetAchievement(info.Id);
                foreach (AchievementChecklistEntry entry in info.ChecklistEntries)
                {
                    if (!CanUpdateChecklist(target, entry, objectId, objectIdAlt))
                        continue;

                    achievement.Data0 |= 1u << (int)entry.Bit;
                    sendUpdate = true;
                }
            }

            if (achievement != null && achievement.IsComplete())
                CompleteAchievement(achievement);

            return sendUpdate;
        }

        /// <summary>
        /// Check if <see cref="AchievementEntry"/> can be updated as <see cref="IPlayer"/> with supplied object ids.
        /// </summary>
        private bool CanUpdateAchievement(IPlayer player, AchievementEntry entry, uint objectId, uint objectIdAlt)
        {
            // TODO: should the server also check PrerequisiteId?
            if (entry.PrerequisiteIdServer != 0u && !PrerequisiteManager.Instance.Meets(player, entry.PrerequisiteIdServer))
                return false;
            
            if (entry.PrerequisiteId != 0u && !PrerequisiteManager.Instance.Meets(player, entry.PrerequisiteId))
                return false;

            // TODO: research PrerequisiteIdObjective and PrerequisiteIdObjectiveAlt

            if (entry.ObjectId != 0u && entry.ObjectId != objectId)
                return false;
            if (entry.ObjectIdAlt != 0u && entry.ObjectIdAlt != objectIdAlt)
                return false;

            return true;
        }

        /// <summary>
        /// Check if <see cref="AchievementChecklistEntry"/> can be updated as <see cref="IPlayer"/> with supplied object ids.
        /// </summary>
        private bool CanUpdateChecklist(IPlayer player, AchievementChecklistEntry entry, uint objectId, uint objectIdAlt)
        {
            if (entry.PrerequisiteId != 0u && !PrerequisiteManager.Instance.Meets(player, entry.PrerequisiteId))
                return false;
            // no checklist entry has PrerequisiteIdAlt set
            if (entry.PrerequisiteIdAlt != 0u && !PrerequisiteManager.Instance.Meets(player, entry.PrerequisiteIdAlt))
                return false;

            if (entry.ObjectId != 0u && entry.ObjectId != objectId)
                return false;
            if (entry.ObjectIdAlt != 0u && entry.ObjectIdAlt != objectIdAlt)
                return false;

            // TODO: Research this case where both values are 0. It's assumed the checklist is checked by ID by a script.
            if (entry.ObjectId == 0u && entry.ObjectIdAlt == 0u)
                return false;

            return true;
        }

        protected virtual void CompleteAchievement(IAchievement achievement)
        {
            achievement.DateCompleted = DateTime.UtcNow;
            AchievementPoints += GetAchievementPoints(achievement.Info);
        }

        /// <summary>
        /// Returns the amount of achievement points earned when completing supplied <see cref="IAchievementInfo"/>.
        /// </summary>
        protected uint GetAchievementPoints(IAchievementInfo info)
        {
            return info.Entry.AchievementPointEnum switch
            {
                1u => 10u,
                2u => 25u,
                3u => 50u,
                _ => 0u
            };
        }

        /// <summary>
        /// Return <see cref="IAchievement"/> with supplied id, if it doesn't exist it will be created.
        /// </summary>
        private IAchievement GetAchievement(ushort id)
        {
            if (achievements.TryGetValue(id, out IAchievement achievement))
                return achievement;

            IAchievementInfo info = GlobalAchievementManager.Instance.GetAchievement(id);
            if (info == null)
                throw new ArgumentException();

            achievement = new Achievement<T>(OwnerId, info);
            achievements.Add(achievement.Id, achievement);
            return achievement;
        }
    }
}
