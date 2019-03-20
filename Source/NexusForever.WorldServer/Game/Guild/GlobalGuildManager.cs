using NexusForever.Database.Character;
using NexusForever.Shared;
using NexusForever.Shared.Database;
using NexusForever.Shared.Game;
using NexusForever.Shared.GameTable;
using NexusForever.WorldServer.Game.CharacterCache;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Game.Social.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NLog;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GuildModel = NexusForever.Database.Character.Model.Guild;

namespace NexusForever.WorldServer.Game.Guild
{
    public sealed partial class GlobalGuildManager : Singleton<GlobalGuildManager>
    {
        private static ILogger log { get; } = LogManager.GetCurrentClassLogger();

        // TODO: move this to the config file
        private const double SaveDuration = 60d;

        /// <summary>
        /// Set up guild operation handlers
        /// </summary>
        private readonly Dictionary<GuildOperation, GuildOperationHandler> guildOperationHandlers
            = new Dictionary<GuildOperation, GuildOperationHandler>();
        private delegate void GuildOperationHandler(WorldSession session, ClientGuildOperation operation, GuildBase guildBase);

        /// <summary>
        /// Id to be assigned to the next created residence.
        /// </summary>
        public ulong NextGuildId => nextGuildId++;
        private ulong nextGuildId;

        private readonly ConcurrentDictionary</*guildId*/ ulong, GuildBase> guilds = new ConcurrentDictionary<ulong, GuildBase>();
        private readonly HashSet<GuildBase> deletedGuilds = new HashSet<GuildBase>();
        private readonly Dictionary<GuildType, uint> maxGuildSize = new Dictionary<GuildType, uint>
        {
            { GuildType.Guild, 40u },
            { GuildType.Circle, 20u },
            { GuildType.ArenaTeam5v5, 9u },
            { GuildType.ArenaTeam3v3, 5u },
            { GuildType.ArenaTeam2v2, 3u },
            { GuildType.WarParty, 80u},
            { GuildType.Community, 5u }
        };

        private UpdateTimer saveTimer = new UpdateTimer(SaveDuration);

        /// <summary>
        /// Initialise the <see cref="GlobalGuildManager"/>, anmd build cache of all existing guilds
        /// </summary>
        public void Initialise()
        {
            nextGuildId = DatabaseManager.Instance.CharacterDatabase.GetNextGuildId() + 1ul;

            foreach (GuildModel guild in DatabaseManager.Instance.CharacterDatabase.GetGuilds())
            {
                switch ((GuildType)guild.Type)
                {
                    case GuildType.Guild:
                        guilds.TryAdd(guild.Id, new Guild(guild.GuildData, guild));
                        break;
                    case GuildType.Circle:
                        guilds.TryAdd(guild.Id, new Circle(guild));
                        break;
                    case GuildType.ArenaTeam2v2:
                    case GuildType.ArenaTeam3v3:
                    case GuildType.ArenaTeam5v5:
                        guilds.TryAdd(guild.Id, new ArenaTeam(guild));
                        break;
                    case GuildType.WarParty:
                        guilds.TryAdd(guild.Id, new WarParty(guild));
                        break;
                    case GuildType.Community:
                        guilds.TryAdd(guild.Id, new Community(guild));
                        break;
                    default:
                        log.Warn($"Guild Type not recognised {(GuildType)guild.Type}");
                        break;
                }
            }

            log.Info($"Initialized {guilds.Count} Guilds.");

            InitialiseGuildOperationHandlers();
        }

        /// <summary>
        /// Initialise all <see cref="GuildOperationHandler"/> that will handle <see cref="GuildOperation"/>
        /// </summary>
        private void InitialiseGuildOperationHandlers()
        {
            IEnumerable<MethodInfo> methods = Assembly.GetExecutingAssembly()
                .GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance));

            foreach (MethodInfo method in methods)
            {
                IEnumerable<GuildOperationHandlerAttribute> attributes = method.GetCustomAttributes<GuildOperationHandlerAttribute>();
                foreach (GuildOperationHandlerAttribute attribute in attributes)
                {
                    #region Debug
                    ParameterInfo[] parameterInfo = method.GetParameters();
                    Debug.Assert(parameterInfo.Length == 3);
                    Debug.Assert(typeof(WorldSession) == parameterInfo[0].ParameterType);
                    Debug.Assert(typeof(ClientGuildOperation) == parameterInfo[1].ParameterType);
                    Debug.Assert(typeof(GuildBase) == parameterInfo[2].ParameterType);
                    #endregion

                    GuildOperationHandler @delegate = (GuildOperationHandler)Delegate.CreateDelegate(typeof(GuildOperationHandler), this, method);
                    guildOperationHandlers.Add(attribute.Operation, @delegate);
                }
            }
        }

        /// <summary>
        /// Executes associated <see cref="GuildOperationHandler"/> if one exists to handle <see cref="GuildOperation"/>
        /// </summary>
        public void HandleGuildOperation(WorldSession session, ClientGuildOperation operation)
        {
            if (guildOperationHandlers.TryGetValue(operation.Operation, out GuildOperationHandler handler))
            {
                if (!GetGuild(operation.GuildId, out GuildBase guild))
                    throw new ArgumentNullException("Guild not found.");

                GuildResult canOperate = HasGuildPermission(guild, session.Player.CharacterId);

                if (canOperate == GuildResult.Success)
                    handler(session, operation, guild);
                else
                    SendGuildResult(session, canOperate, guild);
            }
            else
            {
                log.Info($"GuildOperation {operation.Operation} has no handler implemented.");

                session.EnqueueMessageEncrypted(new ServerChat
                {
                    Channel = ChatChannel.Debug,
                    Name = "GuildManager",
                    Text = $"{operation.Operation} currently not implemented",
                });
            }
        }

        /// <summary>
        /// Checks the Character ID is a member of <see cref="GuildBase"/> and that the <see cref="GuildBase"/> exists
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="characterId"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        private GuildResult HasGuildPermission(GuildBase guild, ulong characterId)
        {
            if (guild == null)
                return GuildResult.NotAGuild;

            if (guild.GetMember(characterId) == null)
                return GuildResult.NotInThatGuild;

            return GuildResult.Success;
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occured.
        /// </summary>
        public void Update(double lastTick)
        {
            saveTimer.Update(lastTick);

            if (saveTimer.HasElapsed)
            {
                var tasks = new List<Task>();
                
                foreach (GuildBase guild in deletedGuilds)
                    tasks.Add(DatabaseManager.Instance.CharacterDatabase.Save(guild.Save));

                foreach (GuildBase guild in guilds.Values)
                    tasks.Add(DatabaseManager.Instance.CharacterDatabase.Save(guild.Save));

                Task.WaitAll(tasks.ToArray());
                deletedGuilds.Clear();

                saveTimer.Reset();
            }
        }

        /// <summary>
        /// Returns <see cref="GuildBase"/> if one exists with the passed guild ID
        /// </summary>
        public GuildBase GetGuild(ulong guildId)
        {
            guilds.TryGetValue(guildId, out GuildBase guild);
            return guild;
        }

        /// <summary>
        /// Returns <see cref="GuildBase"/> if one exists with the passed guild ID, as an <see cref="out"/> parameter
        /// </summary>
        public bool GetGuild(ulong guildId, out GuildBase guild)
        {
            guilds.TryGetValue(guildId, out GuildBase guildBase);
            guild = guildBase;
            if (guild == null)
                return false;

            return true;
        }

        /// <summary>
        /// Entry method to registering any <see cref="GuildBase"/>. Should only be called from the guild handler.
        /// </summary>
        public void RegisterGuild(WorldSession session, ClientGuildRegister clientGuildRegister)
        {
            GuildResult result = GuildResult.Success;
            result = EnsurePlayerNotAtMaxGuildType(session, clientGuildRegister.GuildType);

            if (result == GuildResult.Success)
            {
                if (guilds.FirstOrDefault(i => i.Value.Name.ToLower().Replace(" ", string.Empty) == clientGuildRegister.GuildName.ToLower().Replace(" ", string.Empty)).Key > 0)
                    result = GuildResult.GuildNameUnavailable;
            }

            if (result == GuildResult.Success)
            {
                // TODO: Deduct cost of creation; 10g = Guild, 50p/600 ServiceTokens = Community

                switch (clientGuildRegister.GuildType)
                {
                    case GuildType.Guild:
                        Guild newGuild = CreateGuild(session, clientGuildRegister);
                        if (newGuild != null)
                        {
                            session.Player.GuildId = newGuild.Id;
                            SendPacketsAfterJoin(session, newGuild, GuildResult.YouCreated);
                        }
                        else
                            result = GuildResult.UnableToProcess;
                        break;
                    case GuildType.Circle:
                        GuildBase newCircle = CreateCircle(session, clientGuildRegister);
                        if (newCircle != null)
                            SendPacketsAfterJoin(session, newCircle, GuildResult.YouCreated);
                        else
                            result = GuildResult.UnableToProcess;
                        break;
                    case GuildType.ArenaTeam2v2:
                    case GuildType.ArenaTeam3v3:
                    case GuildType.ArenaTeam5v5:
                        GuildBase newArenaTeam = CreateArenaTeam(session, clientGuildRegister);
                        if (newArenaTeam != null)
                            SendPacketsAfterJoin(session, newArenaTeam, GuildResult.YouCreated);
                        else
                            result = GuildResult.UnableToProcess;
                        break;
                    case GuildType.WarParty:
                        GuildBase newWarParty = CreateWarParty(session, clientGuildRegister);
                        if (newWarParty != null)
                            SendPacketsAfterJoin(session, newWarParty, GuildResult.YouCreated);
                        else
                            result = GuildResult.UnableToProcess;
                        break;
                    case GuildType.Community:
                        GuildBase newCommunity = CreateCommunity(session, clientGuildRegister);
                        if (newCommunity != null)
                            SendPacketsAfterJoin(session, newCommunity, GuildResult.YouCreated);
                        else
                            result = GuildResult.UnableToProcess;
                        break;
                }

                if (result != GuildResult.Success)
                    SendGuildResult(session, result);
            }
            else
                SendGuildResult(session, result);
        }

        /// <summary>
        /// Confirms the user is eligible to join a guild of a given type, that they have not exceeded the allowed amount of that guild type.
        /// </summary>
        private GuildResult EnsurePlayerNotAtMaxGuildType(WorldSession session, GuildType guildType)
        {
            if (guildType == GuildType.Guild && session.Player.GuildId > 0)
                return GuildResult.AtMaxGuildCount;

            if (guildType == GuildType.Circle && session.Player.GuildMemberships.Where(i => guilds[i].Type == GuildType.Circle).Count() >= 5)
                return GuildResult.AtMaxCircleCount;

            if (guildType == GuildType.ArenaTeam2v2 && session.Player.GuildMemberships.Where(i => guilds[i].Type == GuildType.ArenaTeam2v2).Count() >= 1)
                return GuildResult.MaxArenaTeamCount;

            if (guildType == GuildType.ArenaTeam3v3 && session.Player.GuildMemberships.Where(i => guilds[i].Type == GuildType.ArenaTeam3v3).Count() >= 1)
                return GuildResult.MaxArenaTeamCount;

            if (guildType == GuildType.ArenaTeam5v5 && session.Player.GuildMemberships.Where(i => guilds[i].Type == GuildType.ArenaTeam5v5).Count() >= 1)
                return GuildResult.MaxArenaTeamCount;

            if (guildType == GuildType.WarParty && session.Player.GuildMemberships.Where(i => guilds[i].Type == GuildType.WarParty).Count() >= 1)
                return GuildResult.MaxWarPartyCount;

            if (guildType == GuildType.Community && session.Player.GuildMemberships.Where(i => guilds[i].Type == GuildType.Community).Count() >= 1)
                return GuildResult.AtMaxCommunityCount;

            return GuildResult.Success;
        }

        /// <summary>
        /// Returns newly created <see cref="Guild"/> for use when registering
        /// </summary>
        private Guild CreateGuild(WorldSession session, ClientGuildRegister clientGuildRegister)
        {
            var guild = new Guild(session, clientGuildRegister.GuildName, clientGuildRegister.MasterTitle, clientGuildRegister.CouncilTitle, clientGuildRegister.MemberTitle, clientGuildRegister.GuildStandard);
            if (guilds.TryAdd(guild.Id, guild))
                return guild;
            else
                return null;
        }

        /// <summary>
        /// Returns newly created <see cref="Circle"/> for use when registering
        /// </summary>
        private Circle CreateCircle(WorldSession session, ClientGuildRegister clientGuildRegister)
        {
            var guild = new Circle(session, clientGuildRegister.GuildName, clientGuildRegister.MasterTitle, clientGuildRegister.CouncilTitle, clientGuildRegister.MemberTitle);
            guilds.TryAdd(guild.Id, guild);
            return guild;
        }

        /// <summary>
        /// Returns newly created <see cref="WarParty"/> for use when registering
        /// </summary>
        private WarParty CreateWarParty(WorldSession session, ClientGuildRegister clientGuildRegister)
        {
            var guild = new WarParty(session, clientGuildRegister.GuildName, clientGuildRegister.MasterTitle, clientGuildRegister.CouncilTitle, clientGuildRegister.MemberTitle);
            guilds.TryAdd(guild.Id, guild);
            return guild;
        }

        /// <summary>
        /// Returns newly created <see cref="ArenaTeam"/> for use when registering
        /// </summary>
        private ArenaTeam CreateArenaTeam(WorldSession session, ClientGuildRegister clientGuildRegister)
        {
            var guild = new ArenaTeam(session, clientGuildRegister.GuildType, clientGuildRegister.GuildName, clientGuildRegister.MasterTitle, clientGuildRegister.CouncilTitle, clientGuildRegister.MemberTitle);
            guilds.TryAdd(guild.Id, guild);
            return guild;
        }

        /// <summary>
        /// Returns newly created <see cref="Community"/> for use when registering
        /// </summary>
        private Community CreateCommunity(WorldSession session, ClientGuildRegister clientGuildRegister)
        {
            var guild = new Community(session, clientGuildRegister.GuildName, clientGuildRegister.MasterTitle, clientGuildRegister.CouncilTitle, clientGuildRegister.MemberTitle);
            guilds.TryAdd(guild.Id, guild);
            return guild;
        }

        /// <summary>
        /// Handles joining a <see cref="Player"/> to a <see cref="GuildBase"/>, based on a <see cref="GuildInvite"/>. Should only be called from the guild handler.
        /// </summary>
        public void JoinGuild(WorldSession session, GuildInvite guildInvite)
        {
            GuildResult result = GuildResult.Success;

            if (guildInvite == null)
                throw new ArgumentNullException("Guild invite is null.");

            if(!guilds.TryGetValue(guildInvite.GuildId, out GuildBase guild))
                result = GuildResult.NotAGuild;
            else if (guild.GetMemberCount() >= maxGuildSize[guild.Type])
                result = GuildResult.CannotInviteGuildFull;

            if (result == GuildResult.Success)
                result = EnsurePlayerNotAtMaxGuildType(session, guild.Type);

            if (result == GuildResult.Success)
            {
                if (guild.Type == GuildType.Guild)
                    session.Player.GuildId = guild.Id;
                session.Player.GuildMemberships.Add(guild.Id);

                // Update guild with result and new player
                guild.AddMember(new Member(guild.Id, session.Player.CharacterId, guild.GetRank(9), "", guild));
                guild.AnnounceGuildResult(GuildResult.InviteAccepted, referenceText: session.Player.Name);
                guild.OnlineMembers.Add(session.Player.CharacterId);

                // Send Join event to player responding
                SendPacketsAfterJoin(session, guild, GuildResult.YouJoined);
                guild.AnnounceGuildMemberChange(session.Player.CharacterId);
            }
            else
                SendGuildResult(session, result, referenceText: guild.Name);
        }

        /// <summary>
        /// Deletes <see cref="GuildBase"/> with the passed guild ID
        /// </summary>
        /// <param name="guildId"></param>
        public void DeleteGuild(ulong guildId)
        {
            if(!guilds.TryGetValue(guildId, out GuildBase guild))
                throw new ArgumentNullException($"Guild not found with ID {guildId}");

            guild.Delete();
            guilds.Remove(guildId, out GuildBase guildBase);
            deletedGuilds.Add(guildBase);
        }

        /// <summary>
        /// Returns all <see cref="GuildBase"/> that a player is associated with
        /// </summary>
        public IEnumerable<GuildBase> GetMatchingGuilds(ulong characterId)
        {
            return guilds.Values.Where(i => i.GetMember(characterId) != null).OrderBy(x => x.Type).ThenBy(x => x.Id); // Ordering for GuildOperations which operate on index
        }

        /// <summary>
        /// Used to trigger login events for <see cref="Player"/>, and forward them to appropriate <see cref="GuildBase"/>
        /// </summary>
        public void OnPlayerLogin(WorldSession session, Player player)
        {
            var matchingGuilds = GetMatchingGuilds(player.CharacterId);
            foreach (var guild in matchingGuilds)
            {
                if (guild.Type == GuildType.Guild)
                    player.GuildId = guild.Id;

                player.GuildMemberships.Add(guild.Id);
                guild.OnPlayerLogin(player);
            }
            
            ValidateGuildAffiliation(player);

            // TODO: Figure out packet which instructs the client the state of the Holomark.
        }

        /// <summary>
        /// Used to trigger logout events for <see cref="Player"/>, and forward them to appropriate <see cref="GuildBase"/>
        /// </summary>
        public void OnPlayerLogout(Player player)
        {
            foreach (ulong guildId in player.GuildMemberships)
            {
                if(guilds.TryGetValue(guildId, out GuildBase guild))
                    guild.OnPlayerLogout(player);
            }
        }

        /// <summary>
        /// Ensures that the <see cref="Player"/> Guild Affiliation is still valid
        /// </summary>
        private void ValidateGuildAffiliation(Player player)
        {
            if (player.GuildAffiliation <= 0)
                return;

            // Check that the player is allowed to be affiliated with this guild
            if (!player.GuildMemberships.Contains(player.GuildAffiliation))
                player.GuildAffiliation = player.GuildMemberships.Count > 0 ? player.GuildMemberships[0] : 0;

            // Check that the existing or newly affiliated guild exists
            if (player.GuildAffiliation > 0 && !GetGuild(player.GuildAffiliation, out GuildBase guild)) // Used to ensure guild still exists on login
                player.GuildAffiliation = player.GuildMemberships.Count > 0 ? player.GuildMemberships[0] : 0;

            if (player.GuildAffiliation > 0)
                SetGuildHolomark(player, GetGuild(player.GuildAffiliation));
        }

        /// <summary>
        /// Used to send initial packets to the <see cref="Player"/> containing associated guilds
        /// </summary>
        public void SendInitialPackets(WorldSession session)
        {
            List<GuildData> playerGuilds = new List<GuildData>();
            List<GuildMember> playerMemberInfo = new List<GuildMember>();
            List<GuildPlayerLimits> playerUnknowns = new List<GuildPlayerLimits>();
            foreach(ulong guildId in session.Player.GuildMemberships)
            {
                playerGuilds.Add(guilds[guildId].BuildGuildDataPacket());
                var selfPacket = guilds[guildId].GetMember(session.Player.CharacterId).BuildGuildMemberPacket();
                if (session.Player.GuildAffiliation == guildId)
                    selfPacket.Unknown10 = 1;
                playerMemberInfo.Add(selfPacket);
                playerUnknowns.Add(new GuildPlayerLimits());
            }

            int index = 0;
            if (session.Player.GuildAffiliation > 0 && GetGuild(session.Player.GuildAffiliation) != null)
                index = playerGuilds.FindIndex(a => a.GuildId == session.Player.GuildAffiliation);
            ServerGuildInit serverGuildInit = new ServerGuildInit
            {
                NameplateIndex = (uint)index,
                Guilds = playerGuilds,
                Self = playerMemberInfo,
                SelfPrivate = playerUnknowns
            };
            session.EnqueueMessageEncrypted(serverGuildInit);
        }

        /// <summary>
        /// Sends all packets required to instruct the <see cref="Player"/> that they have joined the <see cref="GuildBase"/>. Should only be called by <see cref="JoinGuild(WorldSession, GuildInvite)"/>
        /// </summary>
        private void SendPacketsAfterJoin(WorldSession session, GuildBase newGuild, GuildResult result)
        {
            session.Player.GuildMemberships.Add(newGuild.Id);
            if (session.Player.GuildAffiliation == 0)
                session.Player.GuildAffiliation = newGuild.Id;

            if (newGuild.Type == GuildType.Guild)
                session.Player.GuildHolomarkMask = GuildHolomark.Back;

            SendGuildJoin(session, newGuild.BuildGuildDataPacket(), newGuild.GetMember(session.Player.CharacterId).BuildGuildMemberPacket(), new GuildPlayerLimits());
            SendGuildResult(session, result, newGuild, referenceText: newGuild.Name);
            SendGuildAffiliation(session);
            SendGuildRoster(session, newGuild.GetGuildMembersPackets().ToList(), newGuild.Id);

            // TODO: Figure out packet which instructs the client the state of the Holomark.
        }

        /// <summary>
        /// Sends <see cref="ServerGuildJoin"/> packet to the <see cref="Player"/> with appropriate data
        /// </summary>
        private void SendGuildJoin(WorldSession session, GuildData guildData, GuildMember guildMember, GuildPlayerLimits guildUnknown)
        {
            ServerGuildJoin serverGuildJoin = new ServerGuildJoin
            {
                GuildData = guildData,
                Self = guildMember,
                SelfPrivate = guildUnknown,
                Nameplate = session.Player.GuildAffiliation == guildData.GuildId
            };

            session.EnqueueMessageEncrypted(serverGuildJoin);
        }

        /// <summary>
        /// Sends <see cref="ServerGuildResult"/> packet to the <see cref="Player"/> with appropriate data
        /// </summary>
        private void SendGuildResult(WorldSession session, GuildResult guildResult, GuildBase guild = null, uint referenceId = 0, string referenceText = "")
        {
            ServerGuildResult serverGuildResult = new ServerGuildResult
            {
                Result = guildResult
            };

            if (guild != null)
            {
                serverGuildResult.RealmId = WorldServer.RealmId;
                serverGuildResult.GuildId = guild.Id;
                serverGuildResult.ReferenceId = referenceId;
                serverGuildResult.ReferenceText = referenceText;
            }

            session.EnqueueMessageEncrypted(serverGuildResult);
        }

        /// <summary>
        /// Sends <see cref="ServerGuildRoster"/> packet to the <see cref="Player"/> with appropriate data
        /// </summary>
        private void SendGuildRoster(WorldSession session, List<GuildMember> guildMembers, ulong guildId)
        {
            ServerGuildRoster serverGuildMembers = new ServerGuildRoster
            {
                GuildRealm = WorldServer.RealmId,
                GuildId = guildId,
                GuildMembers = guildMembers,
                Done = true
            };

            session.EnqueueMessageEncrypted(serverGuildMembers);
        }

        /// <summary>
        /// Sends <see cref="ServerEntityGuildAffiliation"/> packet to the <see cref="Player"/> and all surrounding <see cref="Entity"/> with appropriate data
        /// </summary>
        private void SendGuildAffiliation(WorldSession session)
        {
            if (session.Player.GuildAffiliation == 0)
                return;

            GuildBase guild = GetGuild(session.Player.GuildAffiliation);
            if (guild == null)
                return;

            if (guild.GetMember(session.Player.CharacterId) == null)
                return;

            session.Player.EnqueueToVisible(new ServerEntityGuildAffiliation
            {
                UnitId = session.Player.Guid,
                GuildName = guild.Name,
                GuildType = guild.Type
            }, true);
            SetGuildHolomark(session.Player, guild, session, true);
        }

        /// <summary>
        /// Sends <see cref="ServerGuildInvite"/> packet to the <see cref="Player"/> with appropriate data
        /// </summary>
        private void SendPendingInvite(WorldSession session)
        {
            if (session.Player.PendingGuildInvite == null)
                return;

            var guild = GetGuild(session.Player.PendingGuildInvite.GuildId);
            uint taxes = 0;
            if (guild.Type == GuildType.Guild)
            {
                Guild guildInstance = (Guild)guilds[session.Player.PendingGuildInvite.GuildId];
                taxes = guildInstance.Flags;
            }

            ServerGuildInvite serverGuildInvite = new ServerGuildInvite
            {
                GuildName = guild.Name,
                GuildType = guild.Type,
                PlayerName = CharacterManager.Instance.GetCharacterInfo(session.Player.PendingGuildInvite.InviteeId).Name,
                Taxes = taxes
            };

            session.EnqueueMessageEncrypted(serverGuildInvite);
        }

        /// <summary>
        /// Handles removing a <see cref="Player"/> from a <see cref="GuildBase"/> and updating the server and client data appropriately
        /// </summary>
        private void HandlePlayerRemove(WorldSession session, GuildResult result, GuildBase guild, string referenceText = "")
        {
            if (session?.Player == null)
                throw new ArgumentNullException(nameof(session));

            SendGuildResult(session,result, referenceText: referenceText.Length > 0 ? referenceText : session.Player.Name);

            if (session.Player.GuildId == guild.Id)
            {
                session.Player.GuildId = 0;
                session.Player.GuildHolomarkMask = GuildHolomark.None;
                RemoveGuildHolomark(session, true);
            }
            session.Player.GuildMemberships.Remove(guild.Id);
            session.EnqueueMessageEncrypted(new ServerGuildRemove
            {
                RealmId = WorldServer.RealmId,
                GuildId = guild.Id
            });

            if (session.Player.GuildMemberships.Count > 0)
            {
                session.Player.GuildAffiliation = session.Player.GuildMemberships[0];
                SendGuildAffiliation(session);
            }
            else
            {
                session.Player.GuildAffiliation = 0;
                session.Player.EnqueueToVisible(new ServerEntityGuildAffiliation
                {
                    UnitId = session.Player.Guid
                }, true);
            }
        }

        /// <summary>
        /// Removes a Character from all <see cref="GuildBase"/> it was a member of.
        /// </summary>
        public void RemoveCharacterFromAllGuilds(ulong characterId)
        {
            foreach (GuildBase guildBase in GetMatchingGuilds(characterId))
            {
                guildBase.RemoveMember(characterId, out WorldSession memberSession);

                // Let player know they have been removed and update necessary values
                if (memberSession != null)
                    HandlePlayerRemove(memberSession, GuildResult.MemberQuit, guildBase);

                // Announce to guild that player has been removed
                guildBase.SendToOnlineUsers(new ServerGuildMemberRemove
                {
                    RealmId = WorldServer.RealmId,
                    GuildId = guildBase.Id,
                    PlayerIdentity = new TargetPlayerIdentity
                    {
                        RealmId = WorldServer.RealmId,
                        CharacterId = characterId
                    },
                });
                guildBase.AnnounceGuildResult(GuildResult.MemberQuit, referenceText: CharacterManager.Instance.GetCharacterInfo(characterId).Name);
            }
        }

        /// <summary>
        /// Handles the change of guild holomark flags and applies it to the <see cref="Player"/>
        /// </summary>
        public void HandleGuildHolomarkChange(WorldSession session, ClientGuildHolomarkUpdate clientGuildHolomarkUpdate)
        {
            // Player should not be able to do this as part of a guild so just remove any holomarks and return
            if (session.Player.GuildId == 0 || session.Player.GuildAffiliation == 0 || session.Player.GuildId != session.Player.GuildAffiliation) 
            {
                session.Player.GuildHolomarkMask = GuildHolomark.None;
                RemoveGuildHolomark(session, true);
                return;
            }

            if (clientGuildHolomarkUpdate.LeftHidden)
                session.Player.GuildHolomarkMask &= ~GuildHolomark.Left;
            else if (!clientGuildHolomarkUpdate.LeftHidden)
                session.Player.GuildHolomarkMask |= GuildHolomark.Left;

            if (clientGuildHolomarkUpdate.RightHidden)
                session.Player.GuildHolomarkMask &= ~GuildHolomark.Right;
            else if (!clientGuildHolomarkUpdate.RightHidden)
                session.Player.GuildHolomarkMask |= GuildHolomark.Right;

            if (clientGuildHolomarkUpdate.BackHidden)
                session.Player.GuildHolomarkMask &= ~GuildHolomark.Back;
            else if (!clientGuildHolomarkUpdate.BackHidden)
                session.Player.GuildHolomarkMask |= GuildHolomark.Back;

            if (clientGuildHolomarkUpdate.DistanceNear)
                session.Player.GuildHolomarkMask |= GuildHolomark.Near;
            else if (!clientGuildHolomarkUpdate.DistanceNear)
                session.Player.GuildHolomarkMask &= ~GuildHolomark.Near;

            SetGuildHolomark(session.Player, guilds[session.Player.GuildId], session, true);
        }

        /// <summary>
        /// Sets the <see cref="Player"/> <see cref="GuildHolomark"/> and updates local clients if necessary
        /// </summary>
        public void SetGuildHolomark(Player player, GuildBase guildBase, WorldSession session = null, bool isUpdate = false)
        {
            if (guildBase == null)
                throw new ArgumentException($"Guild not found.");
            GuildResult hasPermission = HasGuildPermission(guildBase, player.CharacterId);
            if (hasPermission != GuildResult.Success)
                throw new ArgumentException($"Player does not have permission to use this holomark: {hasPermission}");
            if (!(guildBase is Guild guild))
            {
                RemoveGuildHolomark(session, isUpdate);
                return;
            }

            bool isNear = (player.GuildHolomarkMask & GuildHolomark.Near) != 0;
            Dictionary<ItemSlot, ushort> guildStandardVisuals = new Dictionary<ItemSlot, ushort>
            {
                { ItemSlot.GuildStandardScanLines, (ushort)GameTableManager.Instance.GuildStandardPart.GetEntry(guild.GuildStandard.ScanLines.GuildStandardPartId).ItemDisplayIdStandard },
                { ItemSlot.GuildStandardBackgroundIcon, (ushort)GameTableManager.Instance.GuildStandardPart.GetEntry(guild.GuildStandard.BackgroundIcon.GuildStandardPartId).ItemDisplayIdStandard },
                { ItemSlot.GuildStandardForegroundIcon, (ushort)GameTableManager.Instance.GuildStandardPart.GetEntry(guild.GuildStandard.ForegroundIcon.GuildStandardPartId).ItemDisplayIdStandard },
                { ItemSlot.GuildStandardChest, 5411 }, // 5411 is the magic number found in sniffs
                { ItemSlot.GuildStandardBack, isNear ? (ushort)7163 : (ushort)5580 }, // 7163 = Near, 5580 = Far
                { ItemSlot.GuildStandardShoulderL, isNear ? (ushort)7164 : (ushort)5581 }, // 7164 = Near, 5581 = Far
                { ItemSlot.GuildStandardShoulderR, isNear ? (ushort)7165 : (ushort)5582 } // 7165 = Near, 5582 = Far
            };

            if ((player.GuildHolomarkMask & GuildHolomark.Back) == 0)
                guildStandardVisuals[ItemSlot.GuildStandardBack] = 0;
            if ((player.GuildHolomarkMask & GuildHolomark.Left) == 0)
                guildStandardVisuals[ItemSlot.GuildStandardShoulderL] = 0;
            if ((player.GuildHolomarkMask & GuildHolomark.Right) == 0)
                guildStandardVisuals[ItemSlot.GuildStandardShoulderR] = 0;

            foreach (KeyValuePair<ItemSlot, ushort> guildStandardItem in guildStandardVisuals)
                player.SetAppearance(new ItemVisual { Slot = guildStandardItem.Key, DisplayId = guildStandardItem.Value });

            if (isUpdate && session != null)
            {
                var itemVisuals = new List<ItemVisual>();
                foreach (KeyValuePair<ItemSlot, ushort> guildStandardItem in guildStandardVisuals)
                    itemVisuals.Add(new ItemVisual { Slot = guildStandardItem.Key, DisplayId = guildStandardItem.Value });
                SendPlayerHolomarkChange(session, itemVisuals);
            }
                
        }

        /// <summary>
        /// This removes the <see cref="GuildHolomark"/> when a <see cref="Player"/> changes their affiliation
        /// </summary>
        private void RemoveGuildHolomark(WorldSession session, bool isUpdate = false)
        {
            if (!isUpdate) // We don't need to do this if we're not sending an update.
                return;

            Dictionary<ItemSlot, ushort> guildStandardVisuals = new Dictionary<ItemSlot, ushort>();
            guildStandardVisuals.Add(ItemSlot.GuildStandardBack, 0);
            guildStandardVisuals.Add(ItemSlot.GuildStandardShoulderL, 0);
            guildStandardVisuals.Add(ItemSlot.GuildStandardShoulderR, 0);

            foreach (KeyValuePair<ItemSlot, ushort> guildStandardItem in guildStandardVisuals)
                session.Player.SetAppearance(new ItemVisual { Slot = guildStandardItem.Key, DisplayId = guildStandardItem.Value });

            //var itemVisuals = session.Player.GetAppearance().ToList();
            var itemVisuals = new List<ItemVisual>();
            foreach (KeyValuePair<ItemSlot, ushort> guildStandardItem in guildStandardVisuals)
                itemVisuals.Add(new ItemVisual { Slot = guildStandardItem.Key, DisplayId = guildStandardItem.Value });

            SendPlayerHolomarkChange(session, itemVisuals);
        }

        /// <summary>
        /// Sends <see cref="ServerItemVisualUpdate"/> to all local clients with necessary Holomark changes
        /// </summary>
        private void SendPlayerHolomarkChange(WorldSession session, List<ItemVisual> itemVisuals)
        {
            if (!session.Player.IsLoading)
                session.Player.EnqueueToVisible(new ServerItemVisualUpdate
                {
                    Guid = session.Player.Guid,
                    ItemVisuals = itemVisuals
                }, true);
        }
    }
}
