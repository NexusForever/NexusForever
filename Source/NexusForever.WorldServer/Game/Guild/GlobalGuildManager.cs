using NexusForever.Database;
using NexusForever.Database.Character.Model;
using NexusForever.Shared;
using NexusForever.Shared.Database;
using NexusForever.Shared.Game;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Game.Social.Static;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Game.Guild
{
    public sealed class GlobalGuildManager : AbstractManager<GlobalGuildManager>
    {
        // TODO: move this to the config file
        private const double SaveDuration = 60d;

        /// <summary>
        /// Id to be assigned to the next created guild.
        /// </summary>
        public ulong NextGuildId => nextGuildId++;
        private ulong nextGuildId;

        private readonly Dictionary</*guildId*/ ulong, GuildBase> guilds = new Dictionary</*guildId*/ ulong, GuildBase>();
        private readonly Dictionary<string, ulong> guildNameCache = new Dictionary<string, ulong>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<ulong, List<ulong>> guildMemberCache = new Dictionary<ulong, List<ulong>>();

        private ImmutableDictionary<GuildOperation, (GuildOperationHandlerDelegate, GuildOperationHandlerResultDelegate)> guildOperationHandlers;
        private delegate GuildResultInfo GuildOperationHandlerResultDelegate(GuildBase guild, GuildMember member, Player player, ClientGuildOperation operation);
        private delegate void GuildOperationHandlerDelegate(GuildBase guild, GuildMember member, Player player, ClientGuildOperation operation);

        private readonly UpdateTimer saveTimer = new UpdateTimer(SaveDuration);

        /// <summary>
        /// Initialise the <see cref="GlobalGuildManager"/>, and build cache of all existing guilds
        /// </summary>
        public override GlobalGuildManager Initialise()
        {
            nextGuildId = DatabaseManager.Instance.CharacterDatabase.GetNextGuildId() + 1ul;

            InitialiseGuilds();
            InitialiseGuildOperationHandlers();
            return Instance;
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
                        throw new DatabaseDataException($"Guild type not recognized {(GuildType)model.Type} for guild {model.Id}!");
                }

                guilds.Add(guild.Id, guild);
                guildNameCache.Add(guild.Name, guild.Id);

                // cache character guilds for faster lookup on character login
                List<GuildMember> members = guild.ToList();
                foreach (GuildMember member in members)
                    TrackCharacterGuild(member.CharacterId, guild.Id);

                Log.Trace($"Initialised guild {guild.Name}({guild.Id}) with {members.Count} members.");
            }

            Log.Info($"Initialized {guilds.Count} guilds from the database.");
        }

        /// <summary>
        /// Initialise all <see cref="GuildOperationHandlerDelegate"/> that will handle <see cref="GuildOperation"/>
        /// </summary>
        private void InitialiseGuildOperationHandlers()
        {
            var builder = ImmutableDictionary.CreateBuilder<GuildOperation, (GuildOperationHandlerDelegate, GuildOperationHandlerResultDelegate)> ();
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

                    ParameterExpression guildParameter     = Expression.Parameter(typeof(GuildBase));
                    ParameterExpression memberParameter    = Expression.Parameter(typeof(GuildMember));
                    ParameterExpression playerParameter    = Expression.Parameter(typeof(Player));
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
            Log.Info($"Initilaised {guildOperationHandlers.Count} guild operation handlers.");
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
                foreach (GuildBase guild in guilds.Values.ToList())
                {
                    if (guild.PendingDelete)
                    {
                        guilds.Remove(guild.Id);
                        guildNameCache.Remove(guild.Name);

                        if (guild.PendingCreate)
                            continue;
                    }

                    tasks.Add(DatabaseManager.Instance.CharacterDatabase.Save(guild.Save));
                }
                    
                Task.WaitAll(tasks.ToArray());
                saveTimer.Reset();
            }
        }

        /// <summary>
        /// Returns <see cref="GuildBase"/> with supplied id.
        /// </summary>
        public GuildBase GetGuild(ulong guildId)
        {
            guilds.TryGetValue(guildId, out GuildBase guild);
            return guild;
        }

        /// <summary>
        /// Returns <see cref="GuildBase"/> with supplied name.
        /// </summary>
        public GuildBase GetGuild(string name)
        {
            return guildNameCache.TryGetValue(name, out ulong guildId) ? GetGuild(guildId) : null;
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
                    guild = new Community(name, leaderRankName, councilRankName, memberRankName);
                    break;
                default:
                    throw new ArgumentException();
            }

            guilds.Add(guild.Id, guild);
            guildNameCache.Add(guild.Name, guild.Id);
            return guild;
        }

        /// <summary>
        /// Invoke operation delegate to handle <see cref="GuildOperation"/>.
        /// </summary>
        public void HandleGuildOperation(Player player, ClientGuildOperation operation)
        {
            if (!guildOperationHandlers.TryGetValue(operation.Operation, out (GuildOperationHandlerDelegate, GuildOperationHandlerResultDelegate) handlers))
            {
                Log.Warn($"Received unhandled GuildOperation {operation.Operation}.");

                player.Session.EnqueueMessageEncrypted(new ServerChat
                {
                    Channel = ChatChannelType.Debug,
                    Name    = "GuildManager",
                    Text    = $"{operation.Operation} not implemented!",
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
    }
}
