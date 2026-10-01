using NexusForever.Game.Abstract.PublicEvent;
using NexusForever.Game.Static.PublicEvent;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.PublicEvent;
using NexusForever.Script.Template;
using NexusForever.Shared.Game;

namespace NexusForever.Game.PublicEvent
{
    public class PublicEventObjective : IPublicEventObjective
    {
        public IPublicEventTeam Team { get; private set; }
        public PublicEventObjectiveEntry Entry { get; private set; }
        public PublicEventStatus Status { get; private set; }
        public uint Count { get; private set; }
        public uint DynamicMax { get; private set; }
        public uint Checklist { get; private set; }

        public bool IsBusy { get; private set; }

        private double elapsedTimer;
        private UpdateTimer failureTimer;

        private bool IsChecklist => Entry.PublicEventObjectiveTypeEnum
            is PublicEventObjectiveType.ActivateTargetGroupChecklist
            or PublicEventObjectiveType.TalkToChecklist;

        /// <summary>
        /// Initialise <see cref="PublicEventObjective"/> with suppled <see cref="IPublicEventTeam"/> and <see cref="PublicEventObjectiveEntry"/>.
        /// </summary>
        public void Initialise(IPublicEventTeam team, PublicEventObjectiveEntry entry)
        {
            Team   = team;
            Entry  = entry;
            Status = entry.PublicEventObjectiveFlags.HasFlag(PublicEventObjectiveFlag.InitialObjective)
                ? PublicEventStatus.Active : PublicEventStatus.Inactive;

            // created once and frozen, it only runs while the objective is active
            if (entry.FailureTimeMs > 0)
                failureTimer = new UpdateTimer(TimeSpan.FromMilliseconds(entry.FailureTimeMs), false);

            if (Status == PublicEventStatus.Active)
                StartTimers();
        }

        /// <summary>
        /// Start the elapsed and failure timers, invoked when the objective becomes active.
        /// </summary>
        /// <remarks>
        /// Timers must not run while the objective is inactive, otherwise objectives activated later in the event time out early.
        /// </remarks>
        private void StartTimers()
        {
            elapsedTimer = 0d;
            failureTimer?.Reset();
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            if (IsBusy)
                return;

            if (Status != PublicEventStatus.Active)
                return;

            elapsedTimer += lastTick;

            if (failureTimer == null || !failureTimer.IsTicking)
                return;

            failureTimer.Update(lastTick);
            if (failureTimer.HasElapsed)
            {
                // TimedWin objectives (waits, surviving, timed collecting) are won by lasting until the timer ends, any
                // other timed objective fails
                SetStatus(Entry.PublicEventObjectiveTypeEnum == PublicEventObjectiveType.TimedWin
                    ? PublicEventStatus.Succeeded : PublicEventStatus.Failed);
            }
        }

        private void SetStatus(PublicEventStatus status)
        {
            Status = status;

            if (status == PublicEventStatus.Active)
                StartTimers();
            else
                failureTimer?.Pause();

            // the client restarts the objective timer from the full update, it also includes the status
            if (status == PublicEventStatus.Active)
                BroadcastObjectiveUpdate();
            else
                BroadcastObjectiveStatusUpdate();

            Team.PublicEvent.InvokeScriptCollection<IPublicEventScript>(s => s.OnPublicEventObjectiveStatus(this));
        }

        private void BroadcastObjectiveUpdate()
        {
            BroadcastObjectiveUpdate(new ServerPublicEventObjectiveUpdate
            {
                Objective = Build()
            });
        }

        private void BroadcastObjectiveStatusUpdate()
        {
            BroadcastObjectiveUpdate(new ServerPublicEventObjectiveStatusUpdate
            {
                ObjectiveId     = Entry.Id,
                ObjectiveStatus = BuildObjectiveStatus()
            });
        }

        private void BroadcastObjectiveUpdate(IWritable message)
        {
            if (Entry.LocalizedTextIdOtherTeam == 0)
                Team.Broadcast(message);
            else
            {
                foreach (IPublicEventTeam team in Team.PublicEvent.GetTeams())
                    team.Broadcast(message);
            }
        }

        /// <summary>
        /// Set busy state for the objective.
        /// </summary>
        /// <remarks>
        /// This will pause the objective preventing updates.
        /// </remarks>
        public void SetBusy(bool busy)
        {
            IsBusy = busy;
            BroadcastObjectiveUpdate();
        }

        /// <summary>
        /// Update objective with the supplied count.
        /// </summary>
        public void UpdateObjective(int count)
        {
            if (IsBusy)
                return;

            if (Status != PublicEventStatus.Active)
                return;

            uint oldCount = Count;

            if (IsChecklist)
            {
                uint flag = (uint)(1 << count);
                if ((Checklist & flag) == 0)
                {
                    Checklist |= flag;
                    Count++;
                }
            }
            else
                Count = (uint)Math.Max(0, (int)Count + count);

            if (oldCount != Count)
                BroadcastObjectiveUpdate();

            if (IsComplete())
                SetStatus(PublicEventStatus.Succeeded);
        }

        private bool IsComplete()
        {
            if (Entry.PublicEventObjectiveFlags.HasFlag(PublicEventObjectiveFlag.DynamicObjective))
                return Count >= DynamicMax;

            return Count >= Entry.Count;
        }

        /// <summary>
        /// Activate the objective.
        /// </summary>
        /// <remarks>
        /// This shows the objective to members and allows it to be updated.
        /// </remarks>
        public void ActivateObjective(uint max)
        {
            if (Status != PublicEventStatus.Inactive)
                return;

            DynamicMax = max;
            SetStatus(PublicEventStatus.Active);
        }

        /// <summary>
        /// Reset the objective.
        /// </summary>
        /// <remarks>
        /// This will reset the objective to its initial state allowing it to be activated again.
        /// </remarks>
        public void ResetObjective()
        {
            if (Status != PublicEventStatus.Succeeded)
                return;

            Count      = 0;
            DynamicMax = 0;
            Checklist  = 0;

            SetStatus(PublicEventStatus.Inactive);
        }

        public Network.World.Message.Model.Shared.PublicEventObjective Build()
        {
            return new Network.World.Message.Model.Shared.PublicEventObjective
            {
                ObjectiveId      = Entry.Id,
                ObjectiveStatus  = BuildObjectiveStatus(),
                Busy             = IsBusy,
                ElapsedTimeMs    = (uint)(elapsedTimer * 1000d)
            };
        }

        private Network.World.Message.Model.Shared.PublicEventObjectiveStatus BuildObjectiveStatus()
        {
            return new Network.World.Message.Model.Shared.PublicEventObjectiveStatus
            {
                Status        = Status,
                ObjectiveData = IsChecklist ? Checklist : Count,
                DynamicMax    = DynamicMax
            };
        }
    }
}
