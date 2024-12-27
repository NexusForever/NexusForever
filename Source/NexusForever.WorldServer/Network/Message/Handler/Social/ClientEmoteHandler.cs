using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Social
{
    public class ClientEmoteHandler : IMessageHandler<IWorldSession, ClientEmote>
    {
        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;

        public ClientEmoteHandler(
            IGameTableManager gameTableManager)
        {
            this.gameTableManager = gameTableManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientEmote emote)
        {
            StandState standState = StandState.Stand;
            if (emote.EmoteId != 0)
            {
                EmotesEntry entry = gameTableManager.Emotes.GetEntry(emote.EmoteId);
                if (entry == null)
                    throw new InvalidPacketValueException("HandleEmote: Invalid EmoteId");

                standState = entry.StandState;
            }

            if (emote.EmoteId == 0 && session.Player.IsSitting)
                session.Player.Unsit();

            session.Player.EnqueueToVisible(new ServerEmote
            {
                Guid       = session.Player.Guid,
                StandState = standState,
                EmoteId    = emote.EmoteId
            });
        }
    }
}
