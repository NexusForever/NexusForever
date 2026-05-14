using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Who;
using NexusForever.Network.Message;
using NexusForever.Network.Session;
using NexusForever.Network.World.Message.Model.Who;
using NexusForever.Network.World.Message.Model.Who.Parameter;
using NexusForever.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NexusForever.WorldServer.Network.Message.Handler.Chat
{
    public class ClientWhoRequestHandler : IMessageHandler<IWorldSession, ClientWhoRequest>
    {
        private enum ComboFilterStrategy
        {
            Name,
            Race,
            Class,
            Path,
            Zone,
            Level
        }
        
        private ClientWhoRequest clientWhoRequest;
        private Dictionary<WhoParameterType, Func<WhoParameter, IPlayer, bool>> queryToFunctionMapping;

        /// <summary>
        /// This method interprets the ClientWhoRequest object and returns a list of players who's player attributes match the search query.
        /// </summary>
        /// <param name="requestingSession">IWorldSession information mapped to the player who initiated the who request.</param>
        /// <param name="request"> ClientWhoRequest object that stores the who request parameters that the server received from the client.</param>
        public void HandleMessage(IWorldSession requestingSession, ClientWhoRequest request)
        {
            // WIP: Rāwaho suggested that I use some kind of manager that's thread safe for this operation.
            // WIP: It seems like the PlayerManager is exactly a threadsafe collection of players? Need I do anything more?
            List<IPlayer> playerSessions = PlayerManager.Instance.ToList();

            List<ServerWhoResponse.WhoPlayer> whoResponsePlayerList = new List<ServerWhoResponse.WhoPlayer>();
            WhoParameter? currentRequestParameter = request.Parameters.Count > 0 ? request.Parameters[0] : null;
            clientWhoRequest = request;

            queryToFunctionMapping = new Dictionary<WhoParameterType, Func<WhoParameter, IPlayer, bool>>
            {
                { WhoParameterType.Level, FilterByLevel },
                { WhoParameterType.Race, FilterByRace },
                { WhoParameterType.Path, FilterByPath },
                { WhoParameterType.Class, FilterByClass },
                { WhoParameterType.Zone, FilterByZone },
                { WhoParameterType.Player, FilterByPlayer },
                { WhoParameterType.Guild, FilterByGuild },
                { WhoParameterType.Faction, FilterByFaction },
                { WhoParameterType.Combo, FilterByCombo }
            };

            playerSessions.Aggregate(whoResponsePlayerList, AggregateFilter);

            requestingSession.EnqueueMessageEncrypted(new ServerWhoResponse
            {
                Players = whoResponsePlayerList
            });
        }

        private List<ServerWhoResponse.WhoPlayer> AggregateFilter(List<ServerWhoResponse.WhoPlayer> accumulator, IPlayer playerCandidate)
        {
            Func<WhoParameter, IPlayer, bool>[] queryFunctions = new Func<WhoParameter, IPlayer, bool>[clientWhoRequest.Parameters.Count];
            
            for (int i = 0; i < clientWhoRequest.Parameters.Count; i++)
            {
                WhoParameter whoParameter = clientWhoRequest.Parameters[i];
                queryFunctions[i] = queryToFunctionMapping[whoParameter.Type];
            }

            bool combinedQueryOutcome = false;

            int lastIndex = 0;
            for (int i = 0; i <= clientWhoRequest.ParameterGroupCounts.Count; i++)
            {
                bool singleQueryGroupValue = true;
                int targetIndex = i == clientWhoRequest.ParameterGroupCounts.Count ? clientWhoRequest.Parameters.Count : clientWhoRequest.ParameterGroupCounts[i];
                    
                for (int j = lastIndex; j < targetIndex; j++)
                {
                    if (queryFunctions[j](clientWhoRequest.Parameters[j], playerCandidate) == false)
                    {
                        singleQueryGroupValue = false;
                        break;
                    }
                }

                lastIndex = targetIndex;

                if (singleQueryGroupValue == true)
                {
                    combinedQueryOutcome = true;
                    break;
                }
            }

            if (combinedQueryOutcome == true)
            {
                AddPlayerToList(playerCandidate, accumulator);
            }

            return accumulator;
        }

        private bool FilterByLevel(WhoParameter whoParameter, IPlayer playerCandidate)
        {
            WhoParameterLevel levelData = whoParameter.Data as WhoParameterLevel;
            uint candidateLevel = playerCandidate.Level;
            return candidateLevel >= levelData.BottomLevel && candidateLevel < levelData.TopLevel;
        }

        private bool FilterByRace(WhoParameter whoParameter, IPlayer playerCandidate)
        {
            Race? raceId = null;
            if (whoParameter.Type == WhoParameterType.Race)
            {
                WhoParameterRace raceData = whoParameter.Data as WhoParameterRace;
                raceId = raceData.RaceId;
            }
            else if (whoParameter.Type == WhoParameterType.Combo)
            {
                WhoParameterCombo comboData = whoParameter.Data as WhoParameterCombo;
                raceId = comboData.RaceId;
            }

            return playerCandidate.Race == raceId;
        }

        private bool FilterByPath(WhoParameter whoParameter, IPlayer playerCandidate)
        {
            Game.Static.PlayerPath.Path? pathId = null;
            if (whoParameter.Type == WhoParameterType.Path)
            {
                WhoParameterPath pathData = whoParameter.Data as WhoParameterPath;
                pathId = pathData.PathId;
            }
            else if (whoParameter.Type == WhoParameterType.Combo)
            {
                WhoParameterCombo comboData = whoParameter.Data as WhoParameterCombo;
                pathId = comboData.PathId;
            }

            return playerCandidate.Path == pathId;
        }

        private bool FilterByClass(WhoParameter whoParameter, IPlayer playerCandidate)
        {
            Class? classId = null;
            if (whoParameter.Type == WhoParameterType.Class)
            {
                WhoParameterClass classData = whoParameter.Data as WhoParameterClass;
                classId = classData.ClassId;
            }
            else if (whoParameter.Type == WhoParameterType.Combo)
            {
                WhoParameterCombo comboData = whoParameter.Data as WhoParameterCombo;
                classId = comboData.ClassId;
            }

            return playerCandidate.Class == classId;
        }

        private bool FilterByZone(WhoParameter whoParameter, IPlayer playerCandidate)
        {
            uint? zoneId = null;
            if (whoParameter.Type == WhoParameterType.Zone)
            {
                WhoParameterZone zoneData = whoParameter.Data as WhoParameterZone;
                zoneId = zoneData.WorldZoneId;
            }
            else if (whoParameter.Type == WhoParameterType.Combo)
            {
                WhoParameterCombo comboData = whoParameter.Data as WhoParameterCombo;
                zoneId = comboData.WorldZoneId;
            }

            // WIP: It's not clear how ComboWhoRequests interact with the subzones. I have not been able to get a combo sub-zone request through via the client.
            // For now, if you're in a subzone, we will compare your parent zone id with the search query, since combo search querys seem to only support these.
            uint subZoneId = playerCandidate.Zone.Id;
            uint parentZoneId = playerCandidate.Zone.ParentZoneId;
            if (parentZoneId == 0)
            {
                return playerCandidate.Zone.Id == zoneId;
            }
            else
            {
                return playerCandidate.Zone.ParentZoneId == zoneId;
            }
        }

        private bool FilterByPlayer(WhoParameter whoParameter, IPlayer playerCandidate)
        {
            string? filterString = null;
            if (whoParameter.Type == WhoParameterType.Player)
            {
                WhoParameterPlayer playerData = whoParameter.Data as WhoParameterPlayer;
                filterString = playerData.PlayerName;
            }
            else if(whoParameter.Type == WhoParameterType.Combo)
            {
                WhoParameterCombo comboData = whoParameter.Data as WhoParameterCombo;
                filterString = comboData.SearchString;
            }

            string candidateName = playerCandidate.Name;

            return StringFilter(candidateName, filterString);
        }

        private bool FilterByGuild(WhoParameter whoParameter, IPlayer playerCandidate)
        {
            WhoParameterGuild guildData = whoParameter.Data as WhoParameterGuild;

            if (playerCandidate.GuildManager.Guild == null)
            {
                return false;
            }

            string candidateName = playerCandidate.GuildManager.Guild.Name;
            string filterString = guildData.GuildName;

            return StringFilter(candidateName, filterString);
        }

        private static bool StringFilter(string candidateName, string filterString)
        {
            // WIP: Should we handle string lengths < 3?
            //if (playerData.PlayerName.Length < 3)
            //{
            //    return false;
            //}

            string lowerFilterString = filterString.ToLower();
            string lowerCandidateName = candidateName.ToLower();

            if (lowerCandidateName.IndexOf(lowerFilterString) != -1)
            {
                return true;
            }

            return false;
        }

        private bool FilterByFaction(WhoParameter whoParameter, IPlayer playerCandidate)
        {
            WhoParameterFaction factionData = whoParameter.Data as WhoParameterFaction;
            return playerCandidate.Faction2 == factionData.Faction2Id;
        }

        private bool FilterByCombo(WhoParameter whoParameter, IPlayer playerCandidate)
        {
            WhoParameterCombo comboData = whoParameter.Data as WhoParameterCombo;
            ComboFilterStrategy inferredFilterStrategy = ComboFilterStrategy.Name;

            if (comboData.RaceId != Race.None)
            {
                inferredFilterStrategy = ComboFilterStrategy.Race;
            }
            else if (comboData.ClassId != Class.None)
            {
                inferredFilterStrategy = ComboFilterStrategy.Class;
            }
            else if (comboData.PathId != Game.Static.PlayerPath.Path.None)
            {
                inferredFilterStrategy = ComboFilterStrategy.Path;
            }
            else if (comboData.WorldZoneId != 0)
            {
                inferredFilterStrategy = ComboFilterStrategy.Zone;
            }

            switch(inferredFilterStrategy)
            {
                case ComboFilterStrategy.Name:
                    return FilterByPlayer(whoParameter, playerCandidate);
                case ComboFilterStrategy.Race:
                    return FilterByRace(whoParameter, playerCandidate);
                case ComboFilterStrategy.Class:
                    return FilterByClass(whoParameter, playerCandidate);
                case ComboFilterStrategy.Path:
                    return FilterByPath(whoParameter, playerCandidate);
                case ComboFilterStrategy.Zone:
                    return FilterByZone(whoParameter, playerCandidate);
                default:
                    return false;
            }
        }


        private void AddPlayerToList(IPlayer playerCandidate, List<ServerWhoResponse.WhoPlayer> whoResponsePlayerList)
        {
            whoResponsePlayerList.Add(new()
            {
                Name = playerCandidate.Name,
                Level = playerCandidate.Level,
                Race = playerCandidate.Race,
                Class = playerCandidate.Class,
                Path = playerCandidate.Path,
                Faction = playerCandidate.Faction1,
                Sex = playerCandidate.Sex,
                Zone = playerCandidate.Zone.Id
            });
        }
    }
}