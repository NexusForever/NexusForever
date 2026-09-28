using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Map.Lock;
using NexusForever.Game.Static.Map.Lock;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Instance.Adventure.TheHycrestInsurrection.Script
{
    /// <summary>
    /// Exit Simulation portal (creature 36869): its activate spell 40574 "Leave Simulation - Adventures" (5 s cast) takes
    /// the player out of the adventure, back to where they entered it from.
    /// </summary>
    /// <remarks>
    /// Like retail, leaving through the exit ends the run for that player: their solo instance lock is released, so their
    /// next entry creates a fresh instance. The empty instance unloads after the map unload timer. Players in a match
    /// (group finder) keep the match's lock.
    /// </remarks>
    [ScriptFilterCreatureId((uint)PublicEventCreature.ExitSimulation)]
    public class ExitSimulationEntityScript : IWorldEntityScript, IOwnedScript<IWorldEntity>
    {
        #region Dependency Injection

        private readonly IMapLockManager mapLockManager;

        public ExitSimulationEntityScript(
            IMapLockManager mapLockManager)
        {
            this.mapLockManager = mapLockManager;
        }

        #endregion

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public void OnLoad(IWorldEntity owner)
        {
        }

        /// <summary>
        /// Invoked when a player successfully activates the portal.
        /// </summary>
        public void OnActivateSuccess(IPlayer activator)
        {
            if (activator.ReturnPosition == null)
            {
                // e.g. logged in inside the adventure: the server doesn't know where the player came from
                activator.SendSystemMessage("Leave Simulation: no return location is known for you, use a teleport instead.");
                return;
            }

            if (activator.Map is IMapInstance instance && instance.MapLock?.Type == MapLockType.Solo)
                mapLockManager.RemoveSoloLock(activator.Identity, instance.Entry.Id);

            activator.TeleportTo(activator.ReturnPosition);
        }
    }
}
