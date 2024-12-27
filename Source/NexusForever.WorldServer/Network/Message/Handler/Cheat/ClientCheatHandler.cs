using System;
using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.WorldServer.Command;
using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Network.Message.Handler.Cheat
{
    public class ClientCheatHandler : IMessageHandler<IWorldSession, ClientCheat>
    {
        #region Dependency Injection

        private readonly ILogger<ClientCheatHandler> log;

        private readonly ICommandManager commandManager;

        public ClientCheatHandler(
            ILogger<ClientCheatHandler> log,
            ICommandManager commandManager)
        {
            this.log            = log;
            this.commandManager = commandManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientCheat cheat)
        {
            try
            {
                IWorldEntity target = null;
                if (session.Player.TargetGuid != null)
                    target = session.Player.GetVisible<IWorldEntity>(session.Player.TargetGuid.Value);

                var context = new WorldSessionCommandContext(session, target);
                commandManager.HandleCommand(context, cheat.Message);
            }
            catch (Exception e)
            {
                log.LogWarning(e.Message);
            }
        }
    }
}
