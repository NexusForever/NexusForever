using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Quest.Static;

namespace NexusForever.WorldServer.Game.Quest
{
    public class QuestObjective : IUpdate
    {
        public QuestInfo QuestInfo { get; }
        public QuestObjectiveInfo ObjectiveInfo { get; }

        public byte Index { get; }

        public uint Progress
        {
            get => progress;
            set
            {
                saveMask |= QuestObjectiveSaveMask.Progress;
                progress = value;
            }
        }

        private uint progress;

        public uint? Timer
        {
            get => timer;
            set
            {
                saveMask |= QuestObjectiveSaveMask.Timer;
                timer = value;
            }
        }

        private uint? timer;

        private QuestObjectiveSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="QuestObjective"/> from an existing database model.
        /// </summary>
        public QuestObjective(QuestInfo questInfo, QuestObjectiveInfo objectiveInfo, CharacterQuestObjectiveModel model)
        {
            QuestInfo     = questInfo;
            ObjectiveInfo = objectiveInfo;
            Index         = model.Index;
            progress      = model.Progress;
            timer         = model.Timer;
        }

        /// <summary>
        /// Create a new <see cref="QuestObjective"/> from supplied <see cref="QuestObjectiveEntry"/>.
        /// </summary>
        public QuestObjective(QuestInfo questInfo, QuestObjectiveInfo objectiveInfo, byte index)
        {
            QuestInfo     = questInfo;
            ObjectiveInfo = objectiveInfo;
            Index         = index;

            if (objectiveInfo.Entry.MaxTimeAllowedMS != 0u)
            {
                // TODO
            }

            saveMask = QuestObjectiveSaveMask.Create;
        }

        public void Save(CharacterContext context, ulong characterId)
        {
            if (saveMask == QuestObjectiveSaveMask.None)
                return;

            if ((saveMask & QuestObjectiveSaveMask.Create) != 0)
            {
                context.Add(new CharacterQuestObjectiveModel
                {
                    Id       = characterId,
                    QuestId  = (ushort)QuestInfo.Entry.Id,
                    Index    = Index,
                    Progress = Progress
                });
            }
            else
            {
                var model = new CharacterQuestObjectiveModel
                {
                    Id      = characterId,
                    QuestId = (ushort)QuestInfo.Entry.Id,
                    Index   = Index
                };

                EntityEntry<CharacterQuestObjectiveModel> entity = context.Entry(model);
                if ((saveMask & QuestObjectiveSaveMask.Progress) != 0)
                {
                    model.Progress = Progress;
                    entity.Property(p => p.Progress).IsModified = true;
                }

                if ((saveMask & QuestObjectiveSaveMask.Timer) != 0)
                {
                    // TODO
                }
            }

            saveMask = QuestObjectiveSaveMask.None;
        }

        public void Update(double lastTick)
        {
            // TODO: update timer
        }

        private bool IsDynamic()
        {
            // dynamic objectives have their progress based on percentage rather than count
            return ObjectiveInfo.Type is QuestObjectiveType.KillCreature
                    or QuestObjectiveType.KillTargetGroups
                    or QuestObjectiveType.Unknown15
                    or QuestObjectiveType.KillTargetGroup
                    or QuestObjectiveType.KillCreature2
                && ObjectiveInfo.Entry.Count > 1u
                && !ObjectiveInfo.HasUnknown0200();
        }

        /// <summary>
        /// Return if the objective has been completed.
        /// </summary>
        public bool IsComplete()
        {
            return progress >= GetMaxValue();
        }

        private uint GetMaxValue()
        {
            return IsDynamic() ? 1000u : ObjectiveInfo.Entry.Count;
        }

        /// <summary>
        /// Update object progress with supplied update.
        /// </summary>
        public void ObjectiveUpdate(uint update)
        {
            if (IsDynamic())
                update = (uint)(((float)update / ObjectiveInfo.Entry.Count) * 1000f);

            Progress = Math.Min(progress + update, GetMaxValue());
        }

        /// <summary>
        /// Complete this <see cref="QuestObjective"/>.
        /// </summary>
        public void Complete()
        {
            Progress = GetMaxValue();
        }
    }
}
