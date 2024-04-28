using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement.Command.Position;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Entity.Movement.Spline;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Event;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Example
{
    [ScriptFilterCreatureId(25592)]
    [ScriptFilterIgnore]
    public class NonPlayerScript : INonPlayerScript, IOwnedScript<INonPlayerEntity>
    {
        private static readonly string[] Sayings = new string[]
        {
            "You, again? Here, again? At least you’re predictable.",
            "Way to go Cupcake!",
            "Buy from Lopp, deals never flop!"
        };

        private INonPlayerEntity owner;

        #region Dependency Injection

        private readonly IScriptEventFactory eventFactory;
        private readonly IScriptEventManager eventManager;

        public NonPlayerScript(
            IScriptEventFactory eventFactory,
            IScriptEventManager eventManager)
        {
            this.eventFactory = eventFactory;
            this.eventManager = eventManager;
            this.eventManager.OnScriptEvent += OnEvent;
        }

        #endregion

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public void OnLoad(INonPlayerEntity owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Invoked when <see cref="IScript"/> is unloaded.
        /// </summary>
        public void OnUnload()
        {
            // cancel any pending events
            eventManager.CancelEvents();
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            // update timers for any pending events
            eventManager.Update(lastTick);
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to <see cref="IBaseMap"/>.
        /// </summary>
        public void OnAddToMap(IBaseMap map)
        {
            ScheduleRandomMovement();
            ScheduleSayEvent();
        }

        /// <summary>
        /// Invoked when <see cref="IPositionCommand"/> is finalised.
        /// </summary>
        public void OnPositionEntityCommandFinalise(IPositionCommand command)
        {
            // previous movement has finished, schedule a new one
            ScheduleRandomMovement();
        }

        private void OnEvent(uint? id)
        {
            switch (id)
            {
                case 1:
                    // previous say event has been executed, schedule a new one
                    ScheduleSayEvent();
                    break;
            }
        }

        private void ScheduleRandomMovement()
        {
            // create a new random movement event which will be scheduled for 5 seconds
            var randomMovementEvent = eventFactory.CreateEvent<IEntityRandomMovementEvent>();
            randomMovementEvent.Initialise(owner, 10f, 7f, SplineMode.OneShot);
            eventManager.EnqueueEvent(TimeSpan.FromSeconds(5), randomMovementEvent);
        }

        private void ScheduleSayEvent()
        {
            // create a new say event which will be scheduled between 5 and 10 seconds
            // event has an id of 1, this will allow us to track when it has executed
            var sayEvent = eventFactory.CreateEvent<IEntitySayEvent>();
            sayEvent.Initialise(owner, Sayings[Random.Shared.Next(Sayings.Length - 1)]);
            eventManager.EnqueueEvent(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10), sayEvent, 1);
        }
    }
}