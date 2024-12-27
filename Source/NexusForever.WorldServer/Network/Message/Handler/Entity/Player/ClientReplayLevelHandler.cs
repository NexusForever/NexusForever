using NexusForever.Game.Spell;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity.Player
{
    public class ClientReplayLevelHandler : IMessageHandler<IWorldSession, ClientReplayLevelUp>
    {
        public void HandleMessage(IWorldSession session, ClientReplayLevelUp request)
        {
            session.Player.CastSpell(53378, (byte)(request.Level - 1), new SpellParameters());
        }
    }
}
