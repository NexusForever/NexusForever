using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Misc
{
    public class ClientToggleWeaponsHandler : IMessageHandler<IWorldSession, ClientToggleWeapons>
    {
        public void HandleMessage(IWorldSession session, ClientToggleWeapons toggleWeapons)
        {
            session.Player.Sheathed = toggleWeapons.ToggleState;
        }
    }
}
