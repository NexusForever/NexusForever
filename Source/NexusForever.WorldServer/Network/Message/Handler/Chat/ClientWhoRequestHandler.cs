using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.Session;
using NexusForever.Network.World.Message.Model.Who;
using NexusForever.Network.World.Message.Model.Who.Parameter;
using NexusForever.Shared;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Handler.Chat
{
    public class ClientWhoRequestHandler : IMessageHandler<IWorldSession, ClientWhoRequest>
    {

        private enum FilterStrategy
        {
            Name,
            Race,
            Class,
        }

        private FilterStrategy inferredFilterStrategy;
        public void HandleMessage(IWorldSession session, ClientWhoRequest request)
        {
            INetworkManager<IWorldSession> worldSessions = LegacyServiceProvider.Provider.GetService<INetworkManager<IWorldSession>>();

            var players = new List<ServerWhoResponse.WhoPlayer>();
            var currentRequestParameter = request.Parameters.Count > 0 ? request.Parameters[0] : null;

            // Iterate over sessions for filtering.
            foreach (var sessionKey in worldSessions)
            {
                var sessionPlayer = sessionKey.Player;

                if (currentRequestParameter == null)
                {
                    AddPlayerToList(players, sessionPlayer);
                }
                else if (currentRequestParameter.Type == Game.Static.Who.WhoParameterType.Combo)
                {
                    WhoParameterCombo comboData = currentRequestParameter.Data as WhoParameterCombo;
                    FilterByCombo(players, sessionPlayer, comboData);
                }
            }

            session.EnqueueMessageEncrypted(new ServerWhoResponse
            {
                Players = players
            });
        }

        private void FilterByCombo(List<ServerWhoResponse.WhoPlayer> players, Game.Abstract.Entity.IPlayer sessionPlayer, WhoParameterCombo comboData)
        {
            inferredFilterStrategy = FilterStrategy.Name;

            if (comboData.RaceId != Race.None)
            {
                inferredFilterStrategy = FilterStrategy.Race;
            }
            else if (comboData.ClassId != 0)
            {
                inferredFilterStrategy = FilterStrategy.Class;
            }

            switch (inferredFilterStrategy) 
            {
                case FilterStrategy.Name:
                    FilterByName(players, sessionPlayer, comboData.SearchString, sessionPlayer.Name);
                    break;
                case FilterStrategy.Race:
                    FilterByArgs(players, sessionPlayer, comboData.RaceId, sessionPlayer.Race);
                    break;
                case FilterStrategy.Class:
                    FilterByArgs(players, sessionPlayer, comboData.ClassId, sessionPlayer.Class);
                    break;
            }
        }

        private void FilterByArgs<T>(List<ServerWhoResponse.WhoPlayer> players, Game.Abstract.Entity.IPlayer sessionPlayer, T filterValue, T sessionValue)
        {
            if (EqualityComparer<T>.Default.Equals(filterValue, sessionValue))
            {
                AddPlayerToList(players, sessionPlayer);
            }
        }

        private void FilterByName(List<ServerWhoResponse.WhoPlayer> players, Game.Abstract.Entity.IPlayer sessionPlayer, string comboSearchString, string sessionName)
        {
            // We want to filter in a case insensitive way
            string lowercaseSearch = comboSearchString.ToLower();
            string lowercaseSessionName = sessionName.ToLower();

            if (lowercaseSessionName.IndexOf(lowercaseSearch) != -1)
            {
                AddPlayerToList(players, sessionPlayer);
            }
        }

        private static void AddPlayerToList(List<ServerWhoResponse.WhoPlayer> players, Game.Abstract.Entity.IPlayer sessionPlayer)
        {
            players.Add(new()
            {
                Name = sessionPlayer.Name,
                Level = sessionPlayer.Level,
                Race = sessionPlayer.Race,
                Class = sessionPlayer.Class,
                Path = sessionPlayer.Path,
                Faction = sessionPlayer.Faction1,
                Sex = sessionPlayer.Sex,
                Zone = sessionPlayer.Zone.Id
            });
        }
    }
}
