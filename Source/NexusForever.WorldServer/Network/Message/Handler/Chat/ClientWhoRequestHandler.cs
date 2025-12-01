using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Who;
using NexusForever.Network.Message;
using NexusForever.Network.Session;
using NexusForever.Network.World.Message.Model.Who;
using NexusForever.Network.World.Message.Model.Who.Parameter;
using NexusForever.Shared;
using System;
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
            Path,
            Zone,
            Level
        }

        private IWorldSession requestingSession;
        private FilterStrategy inferredFilterStrategy;

        private readonly bool shouldSearchesIncludeThePlayerInitiatingSearch = true;
        private readonly bool shouldSearchesIncludeOppositeFaction = true;
        public void HandleMessage(IWorldSession requestingSession, ClientWhoRequest request)
        {
            this.requestingSession = requestingSession;
            INetworkManager<IWorldSession> worldSessions = LegacyServiceProvider.Provider.GetService<INetworkManager<IWorldSession>>();

            List<ServerWhoResponse.WhoPlayer> whoResponsePlayerList = new List<ServerWhoResponse.WhoPlayer>();
            WhoParameter? currentRequestParameter = request.Parameters.Count > 0 ? request.Parameters[0] : null;

            // Iterate over sessions (connected clients) for filtering.
            foreach (IWorldSession sessionCandidate in worldSessions)
            {
                if (currentRequestParameter == null)
                {
                    AddPlayerToList(whoResponsePlayerList, sessionCandidate);
                }
                else if (currentRequestParameter.Type == WhoParameterType.Combo)
                {
                    WhoParameterCombo comboData = currentRequestParameter.Data as WhoParameterCombo;
                    FilterByCombo(whoResponsePlayerList, sessionCandidate, comboData);
                }
                else if (currentRequestParameter.Type == WhoParameterType.Level)
                {
                    WhoParameterLevel levelData = currentRequestParameter.Data as WhoParameterLevel;
                    FilterByLevel(whoResponsePlayerList, sessionCandidate, levelData);
                }
            }

            requestingSession.EnqueueMessageEncrypted(new ServerWhoResponse
            {
                Players = whoResponsePlayerList
            });
        }

        private void FilterByLevel(List<ServerWhoResponse.WhoPlayer> whoResponsePlayerList, IWorldSession sessionCandidate, WhoParameterLevel levelData)
        {
            IPlayer candidatePlayer = sessionCandidate.Player;
            uint candidateLevel = candidatePlayer.Level;
            if (candidateLevel >= levelData.BottomLevel && candidateLevel < levelData.TopLevel)
            {
                AddPlayerToList(whoResponsePlayerList, sessionCandidate);
            }
        }

        private void FilterByCombo(List<ServerWhoResponse.WhoPlayer> whoResponsePlayerList, IWorldSession sessionCandidate, WhoParameterCombo comboData)
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
            else if (comboData.WorldZoneId != 0)
            {
                inferredFilterStrategy = FilterStrategy.Zone;
            }

            IPlayer candidatePlayer = sessionCandidate.Player;
            switch (inferredFilterStrategy)
            {
                case FilterStrategy.Name:
                    FilterByName(whoResponsePlayerList, sessionCandidate, comboData.SearchString, candidatePlayer.Name);
                    break;
                case FilterStrategy.Race:
                    FilterByArgs(whoResponsePlayerList, sessionCandidate, comboData.RaceId, candidatePlayer.Race);
                    break;
                case FilterStrategy.Class:
                    FilterByArgs(whoResponsePlayerList, sessionCandidate, comboData.ClassId, candidatePlayer.Class);
                    break;
                case FilterStrategy.Path:
                    FilterByArgs(whoResponsePlayerList, sessionCandidate, comboData.PathId, candidatePlayer.Path);
                    break;
                case FilterStrategy.Zone:
                    FilterByArgs(whoResponsePlayerList, sessionCandidate, comboData.WorldZoneId, candidatePlayer.Zone.Id);
                    break;
            }
        }

        private void FilterByArgs<T>(List<ServerWhoResponse.WhoPlayer> whoResponsePlayerList, IWorldSession sessionCandidate, T filterValue, T candidateValue)
        {
            if (EqualityComparer<T>.Default.Equals(filterValue, candidateValue))
            {
                AddPlayerToList(whoResponsePlayerList, sessionCandidate);
            }
        }

        private void FilterByName(List<ServerWhoResponse.WhoPlayer> whoResponsePlayerList, IWorldSession sessionCandidate, string filterString, string candidateName)
        {
            // We want to filter in a case insensitive way
            string lowerFilterString = filterString.ToLower();
            string lowerCandidateName = candidateName.ToLower();

            if (lowerCandidateName.IndexOf(lowerFilterString) != -1)
            {
                AddPlayerToList(whoResponsePlayerList, sessionCandidate);
            }
        }

        private void AddPlayerToList(List<ServerWhoResponse.WhoPlayer> whoResponsePlayerList, IWorldSession sessionCandidate)
        {
            if (requestingSession.Id == sessionCandidate.Id && (shouldSearchesIncludeThePlayerInitiatingSearch == false))
            {
                // This code exits early if searching party's session ID is matched with one of the filtered client session's IDs.
                return;
            }

            if (requestingSession.Player.Faction1 != sessionCandidate.Player.Faction1 && (shouldSearchesIncludeOppositeFaction == false))
            {
                // This code exits early if searching player's faction does not match the candidate player faction.
                return;
            }

            if (requestingSession.Player.Faction2 != sessionCandidate.Player.Faction2 && (shouldSearchesIncludeOppositeFaction == false))
            {
                // I'm not sure what Faction2 is supposed to be. My best intuition is this might have some kind of relevance in PvP or dueling.
                // In any case, I'm going to add this code here to cover my bases for now.
                // This code exits early if searching player's faction does not match the candidate player faction.
                return;
            }

            IPlayer sessionPlayer = sessionCandidate.Player;
            whoResponsePlayerList.Add(new()
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
