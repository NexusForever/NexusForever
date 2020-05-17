using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Quest.Static;
using NLog;

namespace NexusForever.WorldServer.Game.Quest
{
    public class QuestObjective : IUpdate
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public QuestInfo Info { get; }

        public QuestObjectiveType Type => (QuestObjectiveType)Entry.Type;
        public QuestObjectiveEntry Entry { get; }

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

        private List<uint> targetIds = new List<uint>();

        private QuestObjectiveSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="QuestObjective"/> from an existing database model.
        /// </summary>
        public QuestObjective(QuestInfo info, QuestObjectiveEntry entry, CharacterQuestObjectiveModel model)
        {
            Info     = info;
            Entry    = entry;
            Index    = model.Index;
            progress = model.Progress;
            timer    = model.Timer;

            if (IsChecklist() || UsesTargetGroups())
                BuildTargets();
        }

        /// <summary>
        /// Create a new <see cref="QuestObjective"/> from supplied <see cref="QuestObjectiveEntry"/>.
        /// </summary>
        public QuestObjective(QuestInfo info, QuestObjectiveEntry entry, byte index)
        {
            Info  = info;
            Entry = entry;
            Index = index;

            if (Entry.MaxTimeAllowedMS != 0u)
            {
                // TODO
            }

            if (IsChecklist() || UsesTargetGroups())
                BuildTargets();

            saveMask = QuestObjectiveSaveMask.Create;
        }

        /// <summary>
        /// Builds the target ID list for this <see cref="QuestObjective"/>.
        /// </summary>
        private void BuildTargets()
        {
            uint targetGroupId = Entry.Data > 0 ? Entry.Data : Entry.TargetGroupIdRewardPane;
            if (targetGroupId == 0u)
                throw new InvalidOperationException();

            targetIds = AssetManager.Instance.GetQuestObjectiveTargetIds(Entry.Id).ToList();
            if (targetIds.Count == 0u)
                return;

            if (targetIds.Count < Entry.Count)
                for (int i = targetIds.Count - 1; i < Entry.Count; i++)
                    targetIds.Add(targetIds[0]);
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
                    QuestId  = (ushort)Info.Entry.Id,
                    Index    = Index,
                    Progress = Progress
                });
            }
            else
            {
                var model = new CharacterQuestObjectiveModel
                {
                    Id      = characterId,
                    QuestId = (ushort)Info.Entry.Id,
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
            return (Type == QuestObjectiveType.KillCreature
                || Type == QuestObjectiveType.KillTargetGroups
                || Type == QuestObjectiveType.Unknown15
                || Type == QuestObjectiveType.KillTargetGroup
                || Type == QuestObjectiveType.KillCreature2)
                && Entry.Count > 1u
                && (Entry.Flags & 0x0200) == 0;
        }

        /// <summary>
        /// Return if this <see cref="QuestObjective"/> is a checklist type.
        /// </summary>
        public bool IsChecklist()
        {
            // TODO: Determine other Types that are also Checklists
            return Type == QuestObjectiveType.ActivateTargetGroupChecklist;
        }

        /// <summary>
        /// Return if this <see cref="QuestObjective"/> uses target group data to complete.
        /// </summary>
        public bool UsesTargetGroups()
        {
            // TODO: Determine other Types that are also using Target Groups
            return (Type == QuestObjectiveType.ActivateTargetGroup ||
               Type == QuestObjectiveType.ActivateTargetGroupChecklist ||
               Type == QuestObjectiveType.KillTargetGroup ||
               Type == QuestObjectiveType.KillTargetGroups ||
               Type == QuestObjectiveType.TalkToTargetGroup ||
               Type == QuestObjectiveType.ActivateEntity && Entry.TargetGroupIdRewardPane != 0u);
        }

        /// <summary>
        /// Return if the <see cref="QuestObjective"/> has been completed.
        /// </summary>
        public bool IsComplete()
        {
            return progress >= GetMaxValue();
        }

        /// <summary>
        /// Return if the <see cref="QuestObjective"/> requires the given ID to complete.
        /// </summary>
        public bool IsTarget(uint id)
        {
            return (Entry.Data == id || targetIds.Contains(id));
        }

        private uint GetMaxValue()
        {
            if (IsChecklist())
                return (uint)(1 << (int)Entry.Count) - 1;

            if (IsDynamic())
                return 1000u;

            return Entry.Count;
        }

        /// <summary>
        /// Update object progress with supplied update.
        /// </summary>
        public void ObjectiveUpdate(uint update)
        {
            if (IsChecklist())
                update = (uint)(1 << (int)update);
            else if (IsDynamic())
                update = (uint)(((float)update / Entry.Count) * 1000f);

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
