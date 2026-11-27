using NexusForever.Game.Quest;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Spell;

namespace NexusForever.WorldServer.Network.Message.Handler.Spell
{
    public sealed class ClientCastObjectiveAbilityHandler : IMessageHandler<IWorldSession, ClientCastObjectiveAbility>
    {
        public void HandleMessage(IWorldSession session, ClientCastObjectiveAbility ability)
        {
            WinterfestConversions.StartCast(session.Player, ability.Spell4Id, ability.TargetUnitId);
        }
    }
}
