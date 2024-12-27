using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Guild;
using NexusForever.Game.Static.Guild;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Guild
{
    public class ClientGuildInviteResponseHandler : IMessageHandler<IWorldSession, ClientGuildInviteResponse>
    {
        public void HandleMessage(IWorldSession session, ClientGuildInviteResponse guildInviteResponse)
        {
            IGuildResultInfo info = session.Player.GuildManager.CanAcceptInviteToGuild();
            if (info.Result != GuildResult.Success)
            {
                GuildBase.SendGuildResult(session, info);
                return;
            }

            session.Player.GuildManager.AcceptInviteToGuild(guildInviteResponse.Accepted);
        }
    }
}
