using System.Linq;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Script.Template;

namespace NexusForever.WorldServer.Network.Message.Handler.Chat
{
    public class ClientEmoteHandler : IMessageHandler<IWorldSession, ClientEmote>
    {
        public void HandleMessage(IWorldSession session, ClientEmote emote)
        {
            if (emote.EmoteId == 0 && session.Player.IsSitting)
                session.Player.Unsit();

            session.Player.Emote(emote.EmoteId);

            if (!emote.Silent)
            {
                IPlayer player = session.Player;
                if (emote.Targeted)
                {
                    player.GetVisible<IWorldEntity>(emote.TargetUnitId)
                        ?.InvokeScriptCollection<IWorldEntityScript>(script => script.OnPlayerEmote(player, emote.EmoteId));
                }
                else
                {
                    foreach (IWorldEntity entity in player.GetVisible().OfType<IWorldEntity>().ToArray())
                        entity.InvokeScriptCollection<IWorldEntityScript>(script => script.OnPlayerEmote(player, emote.EmoteId));
                }
            }

            session.Player.EnqueueToVisible(new ServerEmote
            {
                EmotesId     = emote.EmoteId,
                Seed         = emote.Seed,
                SourceUnitId = session.Player.Guid,
                TargetUnitId = emote.TargetUnitId,
                Targeted     = emote.Targeted,
                Silent       = emote.Silent
            });
        }
    }
}
