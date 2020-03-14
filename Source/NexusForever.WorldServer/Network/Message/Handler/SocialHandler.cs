using System;
using System.Collections.Generic;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Command;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Social;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class SocialHandler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static readonly string CommandPrefix = "!";

        [MessageHandler(GameMessageOpcode.ClientChat)]
        public static void HandleChat(WorldSession session, ClientChat chat)
        {
            if (chat.Message.StartsWith(CommandPrefix))
            {
                try
                {
                    CommandManager.Instance.HandleCommand(session, chat.Message, true);
                }
                catch (Exception e)
                {
                    log.Warn(e.Message);
                }
            }
            else
                SocialManager.Instance.HandleClientChat(session, chat);
        }

        [MessageHandler(GameMessageOpcode.ClientEmote)]
        public static void HandleEmote(WorldSession session, ClientEmote emote)
        {
            StandState standState = StandState.Stand;
            if (emote.EmoteId != 0)
            {
                EmotesEntry entry = GameTableManager.Instance.Emotes.GetEntry(emote.EmoteId);
                if (entry == null)
                    throw (new InvalidPacketValueException("HandleEmote: Invalid EmoteId"));

                standState = (StandState)entry.StandState;
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

        [MessageHandler(GameMessageOpcode.ClientWhoRequest)]
        public static void HandleWhoRequest(WorldSession session, ClientWhoRequest request)
        {
            List<ServerWhoResponse.WhoPlayer> players = new List<ServerWhoResponse.WhoPlayer>
            {
                new ServerWhoResponse.WhoPlayer
                {
                    Name = session.Player.Name,
                    Level = session.Player.Level,
                    Race = session.Player.Race,
                    Class = session.Player.Class,
                    Path = session.Player.Path,
                    Faction = Faction.Dominion,
                    Sex = session.Player.Sex,
                    Zone = 1417
                }
            };

            session.EnqueueMessageEncrypted(new ServerWhoResponse
            {
                Players = players
            });
        }
    }
}
