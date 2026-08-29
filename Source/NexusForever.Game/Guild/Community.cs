using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Static.Guild;
using NexusForever.Network.Internal;

namespace NexusForever.Game.Guild
{
    public partial class Community : GuildBase, ICommunity
    {
        public override GuildType Type => GuildType.Community;
        public override uint MaxMembers => 20u;

        public IResidence Residence { get; set; }

        #region Dependency Injection

        private readonly IGlobalResidenceManager globalResidenceManager;

        public Community(
            IRealmContext realmContext,
            IInternalMessagePublisher messagePublisher,
            IGlobalResidenceManager globalResidenceManager)
            : base(realmContext, messagePublisher)
        {
            this.globalResidenceManager = globalResidenceManager;
        }

        #endregion

        /// <summary>
        /// Create a new <see cref="ICommunity"/> using supplied parameters.
        /// </summary>
        public override void Initialise(string guildName, string leaderRankName, string councilRankName, string memberRankName)
        {
            Residence = globalResidenceManager.CreateCommunity(this);

            base.Initialise(guildName, leaderRankName, councilRankName, memberRankName);
        }

        /// <summary>
        /// Set <see cref="ICommunity"/> privacy level.
        /// </summary>
        public void SetCommunityPrivate(bool enabled)
        {
            if (enabled)
                SetFlag(GuildFlag.CommunityPrivate);
            else
                RemoveFlag(GuildFlag.CommunityPrivate);

            SendGuildFlagUpdate();
        }
    }
}
