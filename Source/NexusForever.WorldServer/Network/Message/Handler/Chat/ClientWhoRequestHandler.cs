using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using NexusForever.Game.Abstract.Entity;
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
            Path
        }

        private IWorldSession requestingSession;
        private FilterStrategy inferredFilterStrategy;
        private readonly bool shouldSearchesIncludeThePlayerInitiatingSearch = false;
        public void HandleMessage(IWorldSession requestingSession, ClientWhoRequest request)
        {
            this.requestingSession = requestingSession;
            INetworkManager<IWorldSession> worldSessions = LegacyServiceProvider.Provider.GetService<INetworkManager<IWorldSession>>();

            var players = new List<ServerWhoResponse.WhoPlayer>();
            var currentRequestParameter = request.Parameters.Count > 0 ? request.Parameters[0] : null;

            // Iterate over sessions (connected clients) for filtering.
            foreach (IWorldSession sessionCandidate in worldSessions)
            {
                if (currentRequestParameter == null)
                {
                    AddPlayerToList(players, sessionCandidate);
                }
                else if (currentRequestParameter.Type == Game.Static.Who.WhoParameterType.Combo)
                {
                    WhoParameterCombo comboData = currentRequestParameter.Data as WhoParameterCombo;
                    FilterByCombo(players, sessionCandidate, comboData);
                }
            }

            requestingSession.EnqueueMessageEncrypted(new ServerWhoResponse
            {
                Players = players
            });
        }

        private void FilterByCombo(List<ServerWhoResponse.WhoPlayer> players, IWorldSession sessionCandidate, WhoParameterCombo comboData)
        {
            inferredFilterStrategy = FilterStrategy.Name;

            if (comboData.RaceId != Race.None)
            {
                inferredFilterStrategy = FilterStrategy.Race;
            }
            else if (comboData.ClassId != Class.None)
            {
                inferredFilterStrategy = FilterStrategy.Class;
            }
            else if (comboData.PathId != Game.Static.Entity.Path.None)
            {
                inferredFilterStrategy = FilterStrategy.Path;
            }

            IPlayer candidatePlayer = sessionCandidate.Player;
            switch (inferredFilterStrategy)
            {
                case FilterStrategy.Name:
                    FilterByName(players, sessionCandidate, comboData.SearchString, candidatePlayer.Name);
                    break;
                case FilterStrategy.Race:
                    FilterByArgs(players, sessionCandidate, comboData.RaceId, candidatePlayer.Race);
                    break;
                case FilterStrategy.Class:
                    FilterByArgs(players, sessionCandidate, comboData.ClassId, candidatePlayer.Class);
                    break;
                case FilterStrategy.Path:
                    FilterByArgs(players, sessionCandidate, comboData.ClassId, candidatePlayer.Class);
                    break;
            }
        }

        private void FilterByArgs<T>(List<ServerWhoResponse.WhoPlayer> players, IWorldSession sessionCandidate, T filterValue, T candidateValue)
        {
            if (EqualityComparer<T>.Default.Equals(filterValue, candidateValue))
            {
                AddPlayerToList(players, sessionCandidate);
            }
        }

        private void FilterByName(List<ServerWhoResponse.WhoPlayer> players, IWorldSession sessionCandidate, string filterString, string candidateName)
        {
            // We want to filter in a case insensitive way
            string lowerFilterString = filterString.ToLower();
            string lowerCandidateName = candidateName.ToLower();

            if (lowerCandidateName.IndexOf(lowerFilterString) != -1)
            {
                AddPlayerToList(players, sessionCandidate);
            }
        }

        private void AddPlayerToList(List<ServerWhoResponse.WhoPlayer> players, IWorldSession sessionCandidate)
        {
            if (requestingSession.Id == sessionCandidate.Id && (shouldSearchesIncludeThePlayerInitiatingSearch == false))
            {
                // This code exits early if searching party's session ID is matched with one of the filtered client session's IDs.
                return;
            }

            IPlayer sessionPlayer = sessionCandidate.Player;
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
