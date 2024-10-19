using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Instance.Expedition.EvilFromTheEther.Script
{
    [ScriptFilterCreatureId(71266)]
    [ScriptFilterActivePropId(7059787, 50495937523588)]
    public class PrimaryPowerPlantHallwayDoorEntityScript : AutomaticDoorEntityScript
    {
        #region Dependency Injection

        private readonly IGlobalQuestManager globalQuestManager;

        public PrimaryPowerPlantHallwayDoorEntityScript(
            IGlobalQuestManager globalQuestManager)
        {
            this.globalQuestManager = globalQuestManager;
        }

        #endregion

        /// <summary>
        /// Invoked when <see cref="IDoorEntity"/> is opened.
        /// </summary>
        public override void OnOpenDoor()
        {
            if (door.Map is not IMapInstance mapInstance)
                return;

            ICommunicatorMessage communicatorMessage = globalQuestManager.GetCommunicatorMessage(CommunicatorMessage.CaptainWeir9);
            foreach (IPlayer player in mapInstance.GetPlayers())
                communicatorMessage?.Send(player.Session);
        }
    }
}
