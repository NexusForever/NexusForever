using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Static.Guild;
using NexusForever.Network.Internal;

namespace NexusForever.Game.Guild
{
    public class Circle : GuildBase, ICircle
    {
        public override GuildType Type => GuildType.Circle;
        public override uint MaxMembers => 20u;

        #region Dependency Injection

        public Circle(
            IRealmContext realmContext,
            IInternalMessagePublisher messagePublisher)
            : base(realmContext, messagePublisher)
        {
        }

        #endregion
    }
}
