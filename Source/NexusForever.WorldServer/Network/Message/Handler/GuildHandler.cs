using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Guild;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NLog;
using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class GuildHandler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [MessageHandler(GameMessageOpcode.ClientGuildRegister)]
        public static void HandleGuildRegister(WorldSession session, ClientGuildRegister request)
        {
            GlobalGuildManager.Instance.RegisterGuild(session, request);
        }

        [MessageHandler(GameMessageOpcode.ClientGuildHolomarkUpdate)]
        public static void HandleHolomarkUpdate(WorldSession session, ClientGuildHolomarkUpdate clientGuildHolomarkUpdate)
        {
            //log.Info($"{clientGuildHolomarkUpdate.LeftHidden}, {clientGuildHolomarkUpdate.RightHidden}, {clientGuildHolomarkUpdate.BackHidden}, {clientGuildHolomarkUpdate.DistanceNear}");

            GlobalGuildManager.Instance.HandleGuildHolomarkChange(session, clientGuildHolomarkUpdate);
        }

        [MessageHandler(GameMessageOpcode.ClientGuildOperation)]
        public static void HandleOperation(WorldSession session, ClientGuildOperation clientGuildOperation)
        {
            GlobalGuildManager.Instance.HandleGuildOperation(session, clientGuildOperation);
        }

        [MessageHandler(GameMessageOpcode.ClientGuildInviteResponse)]
        public static void HandeInviteResponse(WorldSession session, ClientGuildInviteResponse clientGuildInviteResponse)
        {
            if (session.Player.PendingGuildInvite == null)
                return;

            if (clientGuildInviteResponse.Accepted)
                GlobalGuildManager.Instance.JoinGuild(session, session.Player.PendingGuildInvite);
            else
            {
                var targetSession = NetworkManager<WorldSession>.Instance.GetSession(i => i.Player?.CharacterId == session.Player.PendingGuildInvite.InviteeId);
                if (targetSession != null)
                    targetSession.EnqueueMessageEncrypted(new ServerGuildResult
                    {
                        RealmId = WorldServer.RealmId,
                        GuildId = session.Player.PendingGuildInvite.GuildId,
                        Result = GuildResult.InviteDeclined,
                        ReferenceText = session.Player.Name
                    });
            }

            session.Player.PendingGuildInvite = null;
        }
    }
}
