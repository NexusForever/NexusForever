using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Housing;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.Social;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Shared;
using NexusForever.Shared.Game;
using NLog;

namespace NexusForever.Game.Guild
{
    public sealed class GlobalGuildManager : Singleton<GlobalGuildManager>, IGlobalGuildManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        // TODO: move this to the config file
        private const double SaveDuration = 60d;

        /// <summary>
        /// Id to be assigned to the next created guild.
        /// </summary>
        public ulong NextGuildId => nextGuildId++;
        private ulong nextGuildId;

        private readonly Dictionary</*guildId*/ ulong, IGuildBase> guilds = new();
        private readonly Dictionary<(GuildType Type, string Name), /*guildId*/ ulong> guildNameCache = new(new GuildNameEqualityComparer());
        private readonly Dictionary</*guildId*/ ulong, List</*memberId*/ ulong>> guildMemberCache = new();

        private ImmutableDictionary<GuildOperation, (GuildOperationHandlerDelegate, GuildOperationHandlerResultDelegate)> guildOperationHandlers;
        private delegate IGuildResultInfo GuildOperationHandlerResultDelegate(IGuildBase guild, IGuildMember member, IPlayer player, ClientGuildOperation operation);
        private delegate void GuildOperationHandlerDelegate(IGuildBase guild, IGuildMember member, IPlayer player, ClientGuildOperation operation);

        private readonly UpdateTimer saveTimer = new(SaveDuration);

        /// <summary>
        /// Initialise the <see cref="IGlobalGuildManager"/>, and build cache of all existing guilds.
        /// </summary>
        public void Initialise()
        {
            if (guilds.Count != 0)
                throw new InvalidOperationException();

            log.Info("Starting guild manager...");

            nextGuildId = DatabaseManager.Instance.GetDatabase<CharacterDatabase>().GetNextGuildId() + 1ul;

            InitialiseGuilds();
            InitialiseGuildOperationHandlers();
        }

        private void InitialiseGuilds()
        {
            foreach (GuildModel model in DatabaseManager.Instance.GetDatabase<CharacterDatabase>().GetGuilds())
            {
                IGuildBase guild;
                switch ((GuildType)model.Type)
                {
                    case GuildType.Guild:
                        guild = new Guild(model);
                        break;
                    case GuildType.Circle:
                        guild = new Circle(model);
                        break;
                    case GuildType.ArenaTeam2v2:
                    case GuildType.ArenaTeam3v3:
                    case GuildType.ArenaTeam5v5:
                        guild = new ArenaTeam(model);
                        break;
                    case GuildType.WarParty:
                        guild = new WarParty(model);
                        break;
                    case GuildType.Community:
                        guild = new Community(model);
                        break;
                    default:
                        throw new DatabaseDataException($"Guild type not recognised {(GuildType)model.Type} for guild {model.Id}!");
                }

                guilds.Add(guild.Id, guild);
                guildNameCache.Add((guild.Type, guild.Name), guild.Id);

                // cache character guilds for faster lookup on character login
                List<IGuildMember> members = guild.ToList();
                foreach (IGuildMember member in members)
                    TrackCharacterGuild(member.CharacterId, guild.Id);

                log.Trace($"Initialised guild {guild.Name}({guild.Id}) with {members.Count} members.");
            }

            log.Info($"Initialized {guilds.Count} guilds from the database.");
        }

        /// <summary>
        /// Initialise all <see cref="GuildOperationHandlerDelegate"/> that will handle <see cref="GuildOperation"/>
        /// </summary>
        private void InitialiseGuildOperationHandlers()
        {
            var builder = ImmutableDictionary.CreateBuilder<GuildOperation, (GuildOperationHandlerDelegate, GuildOperationHandlerResultDelegate)>();
            foreach (MethodInfo method in Assembly.GetExecutingAssembly()
                .GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)))
            {
                IEnumerable<GuildOperationHandlerAttribute> attributes = method.GetCustomAttributes<GuildOperationHandlerAttribute>();
                foreach (GuildOperationHandlerAttribute attribute in attributes)
                {
                    #region Debug
                    ParameterInfo[] parameterInfo = method.GetParameters();
                    Debug.Assert(parameterInfo.Length == 3);
                    Debug.Assert(parameterInfo[0].ParameterType == typeof(IGuildMember));
                    Debug.Assert(parameterInfo[1].ParameterType == typeof(IPlayer));
                    Debug.Assert(parameterInfo[2].ParameterType == typeof(ClientGuildOperation));
                    #endregion

                    ParameterExpression guildParameter = Expression.Parameter(typeof(IGuildBase));
                    ParameterExpression memberParameter = Expression.Parameter(typeof(IGuildMember));
                    ParameterExpression playerParameter = Expression.Parameter(typeof(IPlayer));
                    ParameterExpression operationParameter = Expression.Parameter(typeof(ClientGuildOperation));

                    MethodCallExpression callExpression = Expression.Call(
                        Expression.Convert(guildParameter, method.DeclaringType),
                        method,
                        memberParameter,
                        playerParameter,
                        operationParameter);

                    // guild operation handlers can have 2 different delegates depending on whether the handler returns a result or void
                    if (method.ReturnType == typeof(IGuildResultInfo))
                    {
                        Expression<GuildOperationHandlerResultDelegate> lambdaExpression = Expression.Lambda<GuildOperationHandlerResultDelegate>(
                            callExpression,
                            guildParameter,
                            memberParameter,
                            playerParameter,
                            operationParameter);

                        builder.Add(attribute.Operation, (null, lambdaExpression.Compile()));
                    }
                    else
                    {
                        Expression<GuildOperationHandlerDelegate> lambdaExpression = Expression.Lambda<GuildOperationHandlerDelegate>(
                            callExpression,
                            guildParameter,
                            memberParameter,
                            playerParameter,
                            operationParameter);

                        builder.Add(attribute.Operation, (lambdaExpression.Compile(), null));
                    }
                }
            }

            guildOperationHandlers = builder.ToImmutable();
            log.Info($"Initilaised {guildOperationHandlers.Count} guild operation handlers.");
        }

        /// <summary>
        /// Shutdown <see cref="IGlobalGuildManager"/> and any related resources.
        /// </summary>
        /// <remarks>
        /// This will force save all guilds.
        /// </remarks>
        public void Shutdown()
        {
            log.Info("Shutting down guild manager...");

            SaveGuilds();
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            saveTimer.Update(lastTick);

            if (saveTimer.HasElapsed)
            {
                SaveGuilds();
                saveTimer.Reset();
            }
        }

        private void SaveGuilds()
        {
            var tasks = new List<Task>();
            foreach (GuildBase guild in guilds.Values.ToList())
            {
                if (guild.PendingDelete)
                {
                    guilds.Remove(guild.Id);
                    guildNameCache.Remove((guild.Type, guild.Name));

                    if (guild.PendingCreate)
                        continue;
                }

                tasks.Add(DatabaseManager.Instance.GetDatabase<CharacterDatabase>().Save(guild.Save));
            }

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Validate all <see cref="ICommunity"/> to make sure they have a corresponding residence.
        /// </summary>
        /// <remarks>
        /// This function is mainly here for migrating communities created before the implementation of community plots.
        /// If this happens normally there could be a bigger issue.
        /// </remarks>
        public void ValidateCommunityResidences()
        {
            foreach (IGuildBase guild in guilds.Values)
            {
                if (guild is not Community community)
                    continue;

                if (community.Residence != null)
                    continue;

                community.Residence = GlobalResidenceManager.Instance.CreateCommunity(community);
                log.Warn($"Created new residence {community.Residence.Id} for Community {community.Id} which was missing a residence!");
            }
        }

        /// <summary>
        /// Returns <see cref="IGuildBase"/> with supplied id.
        /// </summary>
        public IGuildBase GetGuild(ulong guildId)
        {
            return guilds.TryGetValue(guildId, out IGuildBase guild) ? guild : null;
        }

        /// <summary>
        /// Returns <see cref="IGuildBase"/> with supplied id.
        /// </summary>
        public T GetGuild<T>(ulong guildId) where T : IGuildBase
        {
            return guilds.TryGetValue(guildId, out IGuildBase guild) ? (T)guild : default;
        }

        /// <summary>
        /// Returns <see cref="IGuildBase"/> with supplied <see cref="GuildType"/> and name.
        /// </summary>
        public IGuildBase GetGuild(GuildType guildType, string name)
        {
            return guildNameCache.TryGetValue((guildType, name), out ulong guildId) ? GetGuild(guildId) : null; 
        }

        /// <summary>
        /// Returns <see cref="IGuildBase"/> with supplied <see cref="GuildType"> and name.
        /// </summary>
        public T GetGuild<T>(GuildType guildType, string name) where T : IGuildBase
        {
            return guildNameCache.TryGetValue((guildType, name), out ulong guildId) ? (T)GetGuild(guildId) : default;
        }

        /// <summary>
        /// Returns a collection of <see cref="IGuildBase"/>'s in which supplied character id belongs to.
        /// </summary>
        /// <remarks>
        /// This should only be used in situations where the local <see cref="IGuildManager"/> is not accessible for a character.
        /// </remarks>
        public IEnumerable<IGuildBase> GetCharacterGuilds(ulong characterId)
        {
            guildMemberCache.TryGetValue(characterId, out List<ulong> characterGuilds);
            foreach (ulong guildId in characterGuilds ?? Enumerable.Empty<ulong>())
                yield return GetGuild(guildId);
        }

        /// <summary>
        /// Track a new guild for the supplied character.
        /// </summary>
        /// <remarks>
        /// Used to notify the global manager that a local manager is tracking a new guild.
        /// </remarks>
        public void TrackCharacterGuild(ulong characterId, ulong guildId)
        {
            if (!guildMemberCache.TryGetValue(characterId, out List<ulong> characterGuilds))
            {
                characterGuilds = new List<ulong>();
                guildMemberCache.Add(characterId, characterGuilds);
            }

            characterGuilds.Add(guildId);
        }

        /// <summary>
        /// Stop tracking an existing guild for the supplied character.
        /// </summary>
        /// <remarks>
        /// Used to notify the global manager that a local manager has stopped tracking an existing guild.
        /// </remarks>
        public void UntrackCharacterGuild(ulong characterId, ulong guildId)
        {
            if (guildMemberCache.TryGetValue(characterId, out List<ulong> characterGuilds))
                characterGuilds.Remove(guildId);
        }

        /// <summary>
        /// Register and return a new <see cref="IGuildBase"/> with the supplied parameters.
        /// </summary>
        /// <remarks>
        /// The new guild does not have a leader, the first <see cref="IPlayer"/> to join will be assigned to the leader.
        /// </remarks>
        public IGuildBase RegisterGuild(GuildType type, string name, string leaderRankName, string councilRankName, string memberRankName, IGuildStandard standard = null)
        {
            IGuildBase guild;
            switch (type)
            {
                case GuildType.Guild:
                    guild = new Guild(name, leaderRankName, councilRankName, memberRankName, standard);
                    break;
                case GuildType.Circle:
                    guild = new Circle(name, leaderRankName, councilRankName, memberRankName);
                    break;
                case GuildType.WarParty:
                    guild = new WarParty(name, leaderRankName, councilRankName, memberRankName);
                    break;
                case GuildType.ArenaTeam2v2:
                case GuildType.ArenaTeam3v3:
                case GuildType.ArenaTeam5v5:
                    guild = new ArenaTeam(type, name, leaderRankName, councilRankName, memberRankName);
                    break;
                case GuildType.Community:
                {
                    var community = new Community(name, leaderRankName, councilRankName, memberRankName);
                    community.Residence = GlobalResidenceManager.Instance.CreateCommunity(community);
                    guild = community;
                    break;
                }
                default:
                    throw new ArgumentException();
            }

            guilds.Add(guild.Id, guild);
            guildNameCache.Add((guild.Type, guild.Name), guild.Id);
            return guild;
        }

        /// <summary>
        /// Invoke operation delegate to handle <see cref="GuildOperation"/>.
        /// </summary>
        public void HandleGuildOperation(IPlayer player, ClientGuildOperation operation)
        {
            if (!guildOperationHandlers.TryGetValue(operation.Operation, out (GuildOperationHandlerDelegate, GuildOperationHandlerResultDelegate) handlers))
            {
                log.Warn($"Received unhandled GuildOperation {operation.Operation}.");

                player.Session.EnqueueMessageEncrypted(new ServerChat
                {
                    Channel  = new Channel
                    {
                        Type = ChatChannelType.Debug
                    },
                    FromName = "GlobalGuildManager",
                    Text     = $"{operation.Operation} not implemented!",
                });

                return;
            }

            IGuildResultInfo info = HandleGuildOperation(handlers, player, operation);
            if (info.Result != GuildResult.Success)
                GuildBase.SendGuildResult(player.Session, info);
        }

        private IGuildResultInfo HandleGuildOperation((GuildOperationHandlerDelegate Delegate, GuildOperationHandlerResultDelegate ResultDelegate) handlers,
            IPlayer player, ClientGuildOperation operation)
        {
            IGuildBase guild = GetGuild(operation.GuildId);
            if (guild == null)
                return new GuildResultInfo(GuildResult.NotAGuild);

            IGuildMember member = guild.GetMember(player.CharacterId);
            if (member == null)
                return new GuildResultInfo(GuildResult.NotInThatGuild);

            if (handlers.Delegate != null)
            {
                handlers.Delegate.Invoke(guild, member, player, operation);
                return new GuildResultInfo(GuildResult.Success);
            }
            else
            {
                IGuildResultInfo info = handlers.ResultDelegate.Invoke(guild, member, player, operation);
                info.GuildId = guild.Id;
                return info;
            }
        }
    }
}
