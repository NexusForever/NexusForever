using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Game.Quest;

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
                AngelPilgrimage.RecordEmote(session.Player, emote.EmoteId, emote.Targeted ? emote.TargetUnitId : null);

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
