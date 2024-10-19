using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Instance.Expedition.EvilFromTheEther.Script
{
    [ScriptFilterCreatureId(71234)]
    public class CrewLogEntityScript : IWorldEntityScript, IOwnedScript<IWorldEntity>
    {
        private IWorldEntity entity;

        #region Dependency Injection

        private readonly IGlobalQuestManager globalQuestManager;

        public CrewLogEntityScript(
            IGlobalQuestManager globalQuestManager)
        {
            this.globalQuestManager = globalQuestManager;
        }

        #endregion

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public void OnLoad(IWorldEntity entity)
        {
            this.entity = entity;
        }

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/> is successfully activated by <see cref="IPlayer"/>.
        /// </summary>
        public void OnActivateSuccess(IPlayer _)
        {
            if (entity.Map is not IMapInstance mapInstance)
                return;

            CommunicatorMessage communicatorMessageId = entity.QuestChecklistIdx switch
            {
                0 => CommunicatorMessage.CrewLog1,
                1 => CommunicatorMessage.CrewLog2,
                2 => CommunicatorMessage.CrewLog3,
                3 => CommunicatorMessage.CrewLog4,
                4 => CommunicatorMessage.CrewLog5,
                5 => CommunicatorMessage.CrewLog6,
                6 => CommunicatorMessage.CrewLog7,
                _ => 0
            };

            ICommunicatorMessage communicatorMessage = globalQuestManager.GetCommunicatorMessage(communicatorMessageId);
            foreach (IPlayer player in mapInstance.GetPlayers())
                communicatorMessage?.Send(player.Session);
        }
    }
}
