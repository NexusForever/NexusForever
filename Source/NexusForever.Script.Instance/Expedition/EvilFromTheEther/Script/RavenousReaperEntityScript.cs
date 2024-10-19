using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Instance.Expedition.EvilFromTheEther.Script
{
    [ScriptFilterCreatureId(71074)]
    public class RavenousReaperEntityScript : INonPlayerScript, IOwnedScript<INonPlayerEntity>
    {
        private INonPlayerEntity entity;

        #region Dependency Injection

        private readonly IGlobalQuestManager globalQuestManager;

        public RavenousReaperEntityScript(
            IGlobalQuestManager globalQuestManager)
        {
            this.globalQuestManager = globalQuestManager;
        }

        #endregion

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public void OnLoad(INonPlayerEntity owner)
        {
            entity = owner;
        }

        /// <summary>
        /// Invoked when <see cref="IUnitEntity"/> is killed.
        /// </summary>
        public void OnDeath()
        {
            if (entity.Map is not IMapInstance mapInstance)
                return;

            ICommunicatorMessage message = globalQuestManager.GetCommunicatorMessage(CommunicatorMessage.CaptainWeir6);
            foreach (IPlayer player in mapInstance.GetPlayers())
                message?.Send(player.Session);
        }
    }
}
