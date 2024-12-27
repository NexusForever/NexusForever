using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Guild
{
    public class ClientGuildHolomarkUpdateHandler : IMessageHandler<IWorldSession, ClientGuildHolomarkUpdate>
    {
        public void HandleMessage(IWorldSession session, ClientGuildHolomarkUpdate guildHolomarkUpdate)
        {
            session.Player.GuildManager.UpdateHolomark(guildHolomarkUpdate.LeftHidden, guildHolomarkUpdate.RightHidden,
                guildHolomarkUpdate.BackHidden, guildHolomarkUpdate.DistanceNear);
        }
    }
}
