using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Command;
using NexusForever.WorldServer.Database.Character;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map;
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
                    CommandManager.HandleCommand(session, chat.Message, true);
                    //CommandManager.ParseCommand(chat.Message, out string command, out string[] parameters);
                    //CommandHandlerDelegate handler = CommandManager.GetCommandHandler(command);
                    //handler?.Invoke(session, parameters);
                }
                catch (Exception e)
                {
                    log.Warn(e.Message);
                }
            }
            else
                SocialManager.HandleClientChat(session, chat);
        }

        [MessageHandler(GameMessageOpcode.ClientEmote)]
        public static void HandleEmote(WorldSession session, ClientEmote emote)
        {
            StandState standState = StandState.Stand;
            if (emote.EmoteId != 0)
            {
                EmotesEntry entry = GameTableManager.Emotes.GetEntry(emote.EmoteId);
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
