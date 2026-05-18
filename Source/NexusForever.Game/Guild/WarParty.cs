using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Static.Guild;
using NexusForever.Network.Internal;

namespace NexusForever.Game.Guild
{
    public class WarParty : GuildBase, IWarParty
    {
        public override GuildType Type => GuildType.WarParty;
        public override uint MaxMembers => 30u;

        #region Dependency Injection

        public WarParty(
            IRealmContext realmContext,
            IInternalMessagePublisher messagePublisher)
            : base(realmContext, messagePublisher)
        {
        }

        #endregion
    }
}
