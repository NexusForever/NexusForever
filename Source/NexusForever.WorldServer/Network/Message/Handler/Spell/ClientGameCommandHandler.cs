using NexusForever.Game.Spell;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Spell;

namespace NexusForever.WorldServer.Network.Message.Handler.Spell
{
    public sealed class ClientGameCommandHandler : IMessageHandler<IWorldSession, ClientGameCommand>
    {
        public void HandleMessage(IWorldSession session, ClientGameCommand command)
        {
            if (command.LimitedActionSetId == 7 && command.AbilityItemIndex == 1)
                SnowblowerSuit.TryChargeFromBar(session.Player, command.TargetUnitId);
        }
    }
}
