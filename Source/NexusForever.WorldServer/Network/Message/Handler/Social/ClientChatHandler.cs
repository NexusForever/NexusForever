using System;
using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Social;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.WorldServer.Command;
using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Network.Message.Handler.Social
{
    public class ClientChatHandler : IMessageHandler<IWorldSession, ClientChat>
    {
        private const string CommandPrefix = "!";

        #region Dependency Injection

        private readonly ILogger<ClientChatHandler> log;

        private readonly ICommandManager commandManager;
        private readonly IGlobalChatManager globalChatManager;

        public ClientChatHandler(
            ILogger<ClientChatHandler> log,
            ICommandManager commandManager,
            IGlobalChatManager globalChatManager)
        {
            this.log               = log;
            this.commandManager    = commandManager;
            this.globalChatManager = globalChatManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientChat chat)
        {
            if (chat.Message.StartsWith(CommandPrefix))
            {
                try
                {
                    IWorldEntity target = null;
                    if (session.Player.TargetGuid != null)
                        target = session.Player.GetVisible<IWorldEntity>(session.Player.TargetGuid.Value);

                    var context = new WorldSessionCommandContext(session, target);
                    commandManager.HandleCommand(context, chat.Message[CommandPrefix.Length..]);
                }
                catch (Exception e)
                {
                    log.LogWarning($"{e.Message}: {e.StackTrace}");
                }
            }
            else
                globalChatManager.HandleClientChat(session.Player, chat);
        }
    }
}
