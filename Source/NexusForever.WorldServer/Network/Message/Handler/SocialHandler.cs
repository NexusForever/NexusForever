using System;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Command;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class SocialHandler
    {
        [MessageHandler(GameMessageOpcode.ClientChat)]
        public static void HandleChat(WorldSession session, ClientChat chat)
        {
            if (chat.Message.StartsWith("!"))
            {
                CommandManager.ParseCommand(chat.Message, out string command, out string[] parameters);
                CommandHandlerDelegate handler = CommandManager.GetCommandHandler(command);
                handler?.Invoke(session, parameters);
            }
        }

        [MessageHandler(GameMessageOpcode.ClientEmote)]
        public static void HandleEmote(WorldSession session, ClientEmote emote)
        {
            uint emoteId = emote.EmoteId;
            uint standState = 0;
            if (emoteId != 0)
            {                
                EmotesEntry entry = GameTableManager.Emotes.GetEntry(emoteId);
                if (entry == null)                
                    throw (new InvalidPacketValueException("HandleEmote: Invalid EmoteId"));

                standState = entry.StandState;
            }
            session.Player.EnqueueToVisible(new ServerEmote
            {
                Guid = session.Player.Guid,
                StandState = standState,
                EmoteId = emoteId
            });
        }
    }
}
