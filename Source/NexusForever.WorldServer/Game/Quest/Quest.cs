using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared;
using NexusForever.Shared.Game;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Quest.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Quest
{
    public class Quest : ISaveCharacter, IUpdate, IEnumerable<QuestObjective>
    {
        public ushort Id => (ushort)Info.Entry.Id;
        public QuestInfo Info { get; }

        public QuestState State
        {
            get => state;
            set
            {
                state = value;
                saveMask |= QuestSaveMask.State;

                player.Session.EnqueueMessageEncrypted(new ServerQuestStateChange
                {
                    QuestId    = Id,
                    QuestState = State
                });
            }
        }

        private QuestState state;

        public QuestFlags Flags
        {
            get => flags;
            set
            {
                flags = value;
                saveMask |= QuestSaveMask.Flags;
            }
        }

        private QuestFlags flags;

        public uint? Timer
        {
            get => timer;
            set
            {
                timer = value;
                saveMask |= QuestSaveMask.Timer;
            }
        }

        private uint? timer;

        public DateTime? Reset
        {
            get => reset;
            set
            {
                reset = value;
                saveMask |= QuestSaveMask.Reset;
            }
        }

        private DateTime? reset;

        /// <summary>
        /// Returns if <see cref="Quest"/> is enqueued to be saved to the database.
        /// </summary>
        public bool PendingCreate => (saveMask & QuestSaveMask.Create) != 0;

        /// <summary>
        /// Returns if <see cref="Quest"/> is enqueued to be deleted from the database.
        /// </summary>
        public bool PendingDelete => (saveMask & QuestSaveMask.Delete) != 0;

        private QuestSaveMask saveMask;

        private readonly Player player;
        private readonly List<QuestObjective> objectives = new List<QuestObjective>();

        private UpdateTimer questTimer;

        /// <summary>
        /// Create a new <see cref="Quest"/> from an existing database model.
        /// </summary>
        public Quest(Player owner, QuestInfo info, CharacterQuestModel model)
        {
            player = owner;
            Info   = info;
            state  = (QuestState)model.State;
            flags  = (QuestFlags)model.Flags;
            timer  = model.Timer;
            reset  = model.Reset;

            if (timer != null)
                questTimer = new UpdateTimer(timer.Value);

            foreach (CharacterQuestObjectiveModel objectiveModel in model.QuestObjective)
                objectives.Add(new QuestObjective(info, info.Objectives[objectiveModel.Index], objectiveModel));
        }

        /// <summary>
        /// Create a new <see cref="Quest"/> from supplied <see cref="QuestInfo"/>.
        /// </summary>
        public Quest(Player owner, QuestInfo info)
        {
            player = owner;
            Info   = info;
            state  = QuestState.Accepted;

            for (byte i = 0; i < info.Objectives.Count; i++)
                objectives.Add(new QuestObjective(info, info.Objectives[i], i));

            saveMask = QuestSaveMask.Create;
        }

        public void InitialiseTimer()
        {
            if (Info.Entry.MaxTimeAllowedMS != 0u)
            {
                questTimer = new UpdateTimer(Info.Entry.MaxTimeAllowedMS / 1000d);
                Timer = (uint)(questTimer.Time * 1000d);
            }

            // TODO: objective timers
        }

        public void Save(CharacterContext context)
        {
            if (saveMask != QuestSaveMask.None)
            {
                if ((saveMask & QuestSaveMask.Create) != 0)
                {
                    context.Add(new CharacterQuestModel
                    {
                        Id      = player.CharacterId,
                        QuestId = Id,
                        State   = (byte)State,
                        Flags   = (byte)Flags,
                        Timer   = Timer,
                        Reset   = Reset
                    });
                }
                else if ((saveMask & QuestSaveMask.Delete) != 0)
                {
                    var model = new CharacterQuestModel
                    {
                        Id      = player.CharacterId,
                        QuestId = Id
                    };

                    context.Entry(model).State = EntityState.Deleted;
                }
                else
                {
                    var model = new CharacterQuestModel
                    {
                        Id      = player.CharacterId,
                        QuestId = Id
                    };

                    EntityEntry<CharacterQuestModel> entity = context.Attach(model);
                    if ((saveMask & QuestSaveMask.State) != 0)
                    {
                        model.State = (byte)State;
                        entity.Property(p => p.State).IsModified = true;
                    }

                    if ((saveMask & QuestSaveMask.Flags) != 0)
                    {
                        model.Flags = (byte)Flags;
                        entity.Property(p => p.Flags).IsModified = true;
                    }

                    if ((saveMask & QuestSaveMask.Reset) != 0)
                    {
                        model.Reset = Reset;
                        entity.Property(p => p.Reset).IsModified = true;
                    }

                    if ((saveMask & QuestSaveMask.Timer) != 0)
                    {
                        model.Timer = Timer;
                        entity.Property(p => p.Timer).IsModified = true;
                    }
                }

                saveMask = QuestSaveMask.None;
            }

            foreach (QuestObjective objective in objectives)
                objective.Save(context, player.CharacterId);
        }

        public void Update(double lastTick)
        {
            if (questTimer != null)
            {
                questTimer.Update(lastTick);
                Timer = (uint)(questTimer.Time * 1000d);

                if (questTimer.HasElapsed)
                {
                    // ran out of time to complete quest
                    State = QuestState.Botched;
                    questTimer = null;
                }
            }
        }

        /// <summary>
        /// Enqueue <see cref="Quest"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete(bool set)
        {
            if (set)
                saveMask |= QuestSaveMask.Delete;
            else
                saveMask &= ~QuestSaveMask.Delete;
        }

        /// <summary>
        /// Returns if <see cref="Quest"/> can be abandoned.
        /// </summary>
        public bool CanAbandon()
        {
            if (State != QuestState.Botched && Info.CannotAbandon())
                return false;

            if (State == QuestState.Achieved && Info.CannotAbandonWhenAchieved())
                return false;

            return true;
        }

        /// <summary>
        /// Returns if <see cref="Quest"/> can be shared with another <see cref="Player"/>.
        /// </summary>
        public bool CanShare()
        {
            if (Info.Entry.QuestShareEnum == 0u)
                return false;

            return State == QuestState.Accepted || State == QuestState.Achieved || State == QuestState.Completed;
        }

        /// <summary>
        /// Update any <see cref="QuestObjective"/>'s with supplied <see cref="QuestObjectiveType"/> and data with progress.
        /// </summary>
        public void ObjectiveUpdate(QuestObjectiveType type, uint data, uint progress)
        {
            foreach (QuestObjective objective in objectives
                .Where(o => o.Entry.Type == (uint)type && o.Entry.Data == data))
            {
                if (!CanUpdateObjective(objective))
                    continue;

                objective.ObjectiveUpdate(progress);
                SendQuestObjectiveUpdate(objective);
            }

            if (objectives.All(o => o.IsComplete()))
                State = QuestState.Achieved;
        }

        private bool CanUpdateObjective(QuestObjective objective)
        {
            // sequential objectives
            if ((objective.Entry.Flags & 0x02) != 0)
            {
                for (int i = 0; i < objective.Index; i++)
                    if (!objectives[i].IsComplete())
                        return false;
            }

            // TODO: client also checks objective flags 1 and 8 in the same function
            return true;
        }

        private void SendQuestObjectiveUpdate(QuestObjective objective)
        {
            player.Session.EnqueueMessageEncrypted(new ServerQuestObjectiveUpdate
            {
                QuestId   = Id,
                Index     = objective.Index,
                Completed = objective.Progress
            });
        }

        public IEnumerator<QuestObjective> GetEnumerator()
        {
            return objectives.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
