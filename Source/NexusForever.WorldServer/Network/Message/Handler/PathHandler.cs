using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class PathHandler
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        [MessageHandler(GameMessageOpcode.ClientPathActivate)]
        public static void HandlePathActivate(WorldSession session, ClientPathActivate clientPathActivate)
        {
            log.Debug($"ClientPathActivate: Path: {clientPathActivate.Path}, UseTokens: {clientPathActivate.UseTokens}");
            Player player = session.Player;

            if (clientPathActivate.UseTokens)
            {
                // TODO: Implement block if not enough tokens
                // TODO: Remove tokens from account and send relevant packet updates
            }
            player.PathManager.ActivatePath(clientPathActivate.Path);
        }

        [MessageHandler(GameMessageOpcode.ClientPathUnlock)]
        public static void HandlePathUnlock(WorldSession session, ClientPathUnlock clientPathUnlock)
        {
            log.Debug($"ClientPathUnlock: Path: {clientPathUnlock.Path}");

            Player player = session.Player;
            // TODO: Handle removing service tokens
            // TODO: Return proper error codes


            // TODO: HasEnoughTokens should be a request to a currency manager of somesort
            bool HasEnoughTokens = true;
            if (HasEnoughTokens)
                player.PathManager.UnlockPath(clientPathUnlock.Path);
        }
    }
}
