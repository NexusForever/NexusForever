﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using NexusForever.Database;
using NexusForever.Database.Character.Model;
using NexusForever.Shared;
using NexusForever.Shared.Database;
using NexusForever.Shared.Game;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Game.Housing;
using NexusForever.WorldServer.Game.Social.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NLog;

namespace NexusForever.WorldServer.Game.Guild
{
    public sealed class GlobalGuildManager : Singleton<GlobalGuildManager>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        // TODO: move this to the config file
        private const double SaveDuration = 60d;

        /// <summary>
        /// Id to be assigned to the next created guild.
        /// </summary>
        public ulong NextGuildId => nextGuildId++;
        private ulong nextGuildId;

        private readonly Dictionary</*guildId*/ ulong, GuildBase> guilds = new();
        private readonly Dictionary<(GuildType, string), ulong> guildNameCache = new(new GuildNameEqualityComparer());
        private readonly Dictionary</*guildId*/ ulong, List</*memberId*/ ulong>> guildMemberCache = new();

        private ImmutableDictionary<GuildOperation, (GuildOperationHandlerDelegate, GuildOperationHandlerResultDelegate)> guildOperationHandlers;
        private delegate GuildResultInfo GuildOperationHandlerResultDelegate(GuildBase guild, GuildMember member, Player player, ClientGuildOperation operation);
        private delegate void GuildOperationHandlerDelegate(GuildBase guild, GuildMember member, Player player, ClientGuildOperation operation);

        private readonly UpdateTimer saveTimer = new(SaveDuration);

        /// <summary>
        /// Initialise the <see cref="GlobalGuildManager"/>, and build cache of all existing guilds
        /// </summary>
        public void Initialise()
        {
            if (guilds.Count != 0)
                throw new InvalidOperationException();

            log.Info("Starting guild manager...");

            nextGuildId = DatabaseManager.Instance.CharacterDatabase.GetNextGuildId() + 1ul;

            InitialiseGuilds();
            InitialiseGuildOperationHandlers();
        }

        private void InitialiseGuilds()
        {
            foreach (GuildModel model in DatabaseManager.Instance.CharacterDatabase.GetGuilds())
            {
                GuildBase guild;
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
                List<GuildMember> members = guild.ToList();
                foreach (GuildMember member in members)
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
                    Debug.Assert(parameterInfo[0].ParameterType == typeof(GuildMember));
                    Debug.Assert(parameterInfo[1].ParameterType == typeof(Player));
                    Debug.Assert(parameterInfo[2].ParameterType == typeof(ClientGuildOperation));
                    #endregion

                    ParameterExpression guildParameter = Expression.Parameter(typeof(GuildBase));
                    ParameterExpression memberParameter = Expression.Parameter(typeof(GuildMember));
                    ParameterExpression playerParameter = Expression.Parameter(typeof(Player));
                    ParameterExpression operationParameter = Expression.Parameter(typeof(ClientGuildOperation));

                    MethodCallExpression callExpression = Expression.Call(
                        Expression.Convert(guildParameter, method.DeclaringType),
                        method,
                        memberParameter,
                        playerParameter,
                        operationParameter);

                    // guild operation handlers can have 2 different delegates depending on whether the handler returns a result or void
                    if (method.ReturnType == typeof(GuildResultInfo))
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
        /// Shutdown <see cref="GlobalGuildManager"/> and any related resources.
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

                tasks.Add(DatabaseManager.Instance.CharacterDatabase.Save(guild.Save));
            }

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Validate all <see cref="Community"/> to make sure they have a corresponding residence.
        /// </summary>
        /// <remarks>
        /// This function is mainly here for migrating communities created before the implementation of community plots.
        /// If this happens normally there could be a bigger issue.
        /// </remarks>
        public void ValidateCommunityResidences()
        {
            foreach (GuildBase guild in guilds.Values)
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
        /// Returns <see cref="GuildBase"/> with supplied id.
        /// </summary>
        public GuildBase GetGuild(ulong guildId)
        {
            return guilds.TryGetValue(guildId, out GuildBase guild) ? guild : null;
        }

        /// <summary>
        /// Returns <see cref="GuildBase"/> with supplied id.
        /// </summary>
        public T GetGuild<T>(ulong guildId) where T : GuildBase
        {
            return guilds.TryGetValue(guildId, out GuildBase guild) ? (T)guild : null;
        }

        /// <summary>
        /// Returns <see cref="GuildBase"/> with supplied <see cref="GuildType"/> and name.
        /// </summary>
        public GuildBase GetGuild(GuildType guildType, string name)
        {
            return guildNameCache.TryGetValue((guildType, name), out ulong guildId) ? GetGuild(guildId) : null;
        }

        /// <summary>
        /// Returns <see cref="GuildBase"/> with supplied <see cref="GuildType"> and name.
        /// </summary>
        public T GetGuild<T>(GuildType guildType, string name) where T : GuildBase
        {
            return guildNameCache.TryGetValue((guildType, name), out ulong guildId) ? (T)GetGuild(guildId) : null;
        }

        /// <summary>
        /// Returns a collection of <see cref="GuildBase"/>'s in which supplied character id belongs to.
        /// </summary>
        /// <remarks>
        /// This should only be used in situations where the local <see cref="GuildManager"/> is not accessible for a character.
        /// </remarks>
        public IEnumerable<GuildBase> GetCharacterGuilds(ulong characterId)
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
        /// Register and return a new <see cref="GuildBase"/> with the supplied parameters.
        /// </summary>
        /// <remarks>
        /// The new guild does not have a leader, the first <see cref="Player"/> to join will be assigned to the leader.
        /// </remarks>
        public GuildBase RegisterGuild(GuildType type, string name, string leaderRankName, string councilRankName, string memberRankName, GuildStandard standard = null)
        {
            GuildBase guild;
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
        public void HandleGuildOperation(Player player, ClientGuildOperation operation)
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

            GuildResultInfo info = HandleGuildOperation(handlers, player, operation);
            if (info.Result != GuildResult.Success)
                GuildBase.SendGuildResult(player.Session, info);
        }

        private GuildResultInfo HandleGuildOperation((GuildOperationHandlerDelegate Delegate, GuildOperationHandlerResultDelegate ResultDelegate) handlers,
            Player player, ClientGuildOperation operation)
        {
            GuildBase guild = GetGuild(operation.GuildId);
            if (guild == null)
                return new GuildResultInfo(GuildResult.NotAGuild);

            GuildMember member = guild.GetMember(player.CharacterId);
            if (member == null)
                return new GuildResultInfo(GuildResult.NotInThatGuild);

            if (handlers.Delegate != null)
            {
                handlers.Delegate.Invoke(guild, member, player, operation);
                return new GuildResultInfo(GuildResult.Success);
            }
            else
            {
                GuildResultInfo info = handlers.ResultDelegate.Invoke(guild, member, player, operation);
                info.GuildId = guild.Id;
                return info;
            }
        }
        private class GuildNameEqualityComparer : IEqualityComparer<(GuildType, string)>
        {
            public bool Equals((GuildType, string) x, (GuildType, string) y)
            {
                if (x.Item1 == y.Item1)
                {
                    if (ReferenceEquals(x.Item2, y.Item2))      return true;
                    if (x.Item2 is null)                        return false;
                    if (y.Item2 is null)                        return false;
                    if (x.Item2.GetType() != y.Item2.GetType()) return false;

                    return string.Equals(x.Item2, y.Item2, StringComparison.InvariantCultureIgnoreCase);
                }

                return false;
            }

            public int GetHashCode([DisallowNull] (GuildType, string) obj)
            {
                return base.GetHashCode();
            }
        }
    }
}
