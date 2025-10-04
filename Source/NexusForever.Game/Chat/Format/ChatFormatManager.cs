using System.Linq.Expressions;
using NexusForever.Game.Abstract.Chat.Format;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Social;
using NexusForever.Network.Internal.Message.Chat.Shared.Format;
using NexusForever.Network.Internal.Message.Chat.Shared.Format.Model;
using NexusForever.Network.World.Chat;
using NexusForever.Network.World.Chat.Model;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Chat.Format
{
    public class ChatFormatManager : IChatFormatManager
    {
        private readonly Dictionary<ChatFormatType, Type> internalFormatterTypes = [];
        private readonly Dictionary<ChatFormatType, Func<object, IPlayer, IChatFormatModel, IChatChannelTextFormatModel>> internalFormatters = [];

        private readonly Dictionary<ChatFormatType, Type> networkFormatterTypes = [];
        private readonly Dictionary<ChatFormatType, Func<object, IChatChannelTextFormatModel, IChatFormatModel>> networkFormatters = [];

        private readonly Dictionary<ChatFormatType, Type> localFormatterTypes = [];
        private readonly Dictionary<ChatFormatType, Func<object, IPlayer, IChatFormatModel, IChatFormatModel>> localFormatters = [];

        #region Dependency Injection

        private readonly IServiceProvider serviceProvider;

        public ChatFormatManager(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        #endregion

        public void Initialise()
        {
            InitialiseInternal();
            InitialiseNetwork();
            InitialiseLocal();
        }

        private void InitialiseInternal()
        {
            RegisterInternalFormatter<ChatFormatItemGuid>(ChatFormatType.ItemGuid);
            RegisterInternalFormatter<ChatFormatItemId>(ChatFormatType.ItemId);
            RegisterInternalFormatter<ChatFormatQuestId>(ChatFormatType.QuestId);
        }

        private void InitialiseNetwork()
        {
            RegisterNetworkFormatter<ChatChannelTextItemIdFormat>(ChatFormatType.ItemId);
            RegisterNetworkFormatter<ChatChannelTextQuestIdFormat>(ChatFormatType.QuestId);
        }

        private void InitialiseLocal()
        {
            RegisterLocalFormatter<ChatFormatItemGuid>(ChatFormatType.ItemGuid);
        }

        private void RegisterInternalFormatter<T>(ChatFormatType type) where T : IChatFormatModel
        {
            var formatterType = typeof(IInternalChatFormatter<>).MakeGenericType(typeof(T));
            var methodInfo    = formatterType.GetMethod(nameof(IInternalChatFormatter<IChatFormatModel>.ToInternal));

            var formatterParameter = Expression.Parameter(typeof(object), "formatter");
            var playerParameter    = Expression.Parameter(typeof(IPlayer), "player");
            var modelParameter     = Expression.Parameter(typeof(IChatFormatModel), "model");

            var formatterConvert = Expression.Convert(formatterParameter, formatterType);
            var modelConvert     = Expression.Convert(modelParameter, typeof(T));
            var callExpression   = Expression.Call(formatterConvert, methodInfo, playerParameter, modelConvert);
            var lambdaExpression = Expression.Lambda<Func<object, IPlayer, IChatFormatModel, IChatChannelTextFormatModel>>(callExpression, formatterParameter, playerParameter, modelParameter);

            internalFormatterTypes.Add(type, formatterType);
            internalFormatters.Add(type, lambdaExpression.Compile());
        }

        private void RegisterNetworkFormatter<T>(ChatFormatType type) where T : IChatChannelTextFormatModel
        {
            var formatterType = typeof(INetworkChatFormatter<>).MakeGenericType(typeof(T));
            var methodInfo    = formatterType.GetMethod(nameof(INetworkChatFormatter<IChatChannelTextFormatModel>.ToNetwork));

            var formatterParameter = Expression.Parameter(typeof(object), "formatter");
            var modelParameter     = Expression.Parameter(typeof(IChatChannelTextFormatModel), "model");

            var formatterConvert = Expression.Convert(formatterParameter, formatterType);
            var modelConvert     = Expression.Convert(modelParameter, typeof(T));
            var callExpression   = Expression.Call(formatterConvert, methodInfo, modelConvert);
            var lambdaExpression = Expression.Lambda<Func<object, IChatChannelTextFormatModel, IChatFormatModel>>(callExpression, formatterParameter, modelParameter);

            networkFormatterTypes.Add(type, formatterType);
            networkFormatters.Add(type, lambdaExpression.Compile());
        }

        private void RegisterLocalFormatter<T>(ChatFormatType type) where T : IChatFormatModel
        {
            var formatterType = typeof(ILocalChatFormatter<>).MakeGenericType(typeof(T));
            var methodInfo    = formatterType.GetMethod(nameof(ILocalChatFormatter<IChatFormatModel>.ToLocal));

            var formatterParameter = Expression.Parameter(typeof(object), "formatter");
            var playerParameter    = Expression.Parameter(typeof(IPlayer), "player");
            var modelParameter     = Expression.Parameter(typeof(IChatFormatModel), "model");

            var formatterConvert = Expression.Convert(formatterParameter, formatterType);
            var modelConvert     = Expression.Convert(modelParameter, typeof(T));
            var callExpression   = Expression.Call(formatterConvert, methodInfo, playerParameter, modelConvert);
            var lambdaExpression = Expression.Lambda<Func<object, IPlayer, IChatFormatModel, IChatFormatModel>>(callExpression, formatterParameter, playerParameter, modelParameter);

            localFormatterTypes.Add(type, formatterType);
            localFormatters.Add(type, lambdaExpression.Compile());
        }

        /// <summary>
        /// Converts a collection of <see cref="ChatFormat"/> to a collection of <see cref="ChatChannelTextFormat"/>.
        /// </summary>
        /// <param name="player">Player the chat formats belong to.</param>
        /// <param name="formats">A collection of <see cref="ChatFormat"/>'s to be converted.</param>
        public IEnumerable<ChatChannelTextFormat> ToInternal(IPlayer player, IEnumerable<ChatFormat> formats)
        {
            foreach (ChatFormat format in formats)
            {
                var internalFormat = new ChatChannelTextFormat
                {
                    StartIndex = format.StartIndex,
                    StopIndex  = format.StopIndex
                };

                if (!internalFormatterTypes.TryGetValue(format.Type, out Type type)
                    || !internalFormatters.TryGetValue(format.Type, out Func<object, IPlayer, IChatFormatModel, IChatChannelTextFormatModel> formatterDelegate))
                    throw new NotImplementedException();

                object formatter = serviceProvider.GetService(type);
                if (formatter == null)
                    throw new NotImplementedException($"Formatter for type {format.Type} not registered.");

                internalFormat.Model = formatterDelegate(formatter, player, format.Model);
                internalFormat.Type  = internalFormat.Model.Type;

                yield return internalFormat;
            }
        }

        /// <summary>
        /// Converts a collection of <see cref="ChatChannelTextFormat"/> to a collection of <see cref="ChatFormat"/>.
        /// </summary>
        /// <param name="formats">A collection of <see cref="ChatChannelTextFormat"/>'s to be converted.</param>
        public IEnumerable<ChatFormat> ToNetwork(IEnumerable<ChatChannelTextFormat> formats)
        {
            foreach (ChatChannelTextFormat format in formats)
            {
                var networkFormat = new ChatFormat
                {
                    StartIndex = format.StartIndex,
                    StopIndex  = format.StopIndex
                };

                if (!networkFormatterTypes.TryGetValue(format.Type, out Type type)
                    || !networkFormatters.TryGetValue(format.Type, out Func<object, IChatChannelTextFormatModel, IChatFormatModel> formatterDelegate))
                    throw new NotImplementedException();

                object formatter = serviceProvider.GetService(type);
                if (formatter == null)
                    throw new NotImplementedException($"Formatter for type {format.Type} not registered.");

                networkFormat.Model = formatterDelegate(formatter, format.Model);
                networkFormat.Type  = networkFormat.Model.Type;

                yield return networkFormat;
            }
        }

        /// <summary>
        /// Converts a collection of <see cref="ChatFormat"/> to a collection of <see cref="ChatFormat"/>.
        /// </summary>
        /// <param name="player">Player the chat formats belong to.</param>
        /// <param name="formats">A collection of <see cref="ChatFormat"/>'s to be converted.</param>
        public IEnumerable<ChatFormat> ToLocal(IPlayer player, IEnumerable<ChatFormat> formats)
        {
            foreach (ChatFormat format in formats)
            {
                if (!localFormatterTypes.TryGetValue(format.Type, out Type type)
                    || !localFormatters.TryGetValue(format.Type, out Func<object, IPlayer, IChatFormatModel, IChatFormatModel> formatterDelegate))
                {
                    yield return format;
                    continue;
                }

                var localFormat = new ChatFormat
                {
                    StartIndex = format.StartIndex,
                    StopIndex  = format.StopIndex
                };

                object formatter = serviceProvider.GetService(type);
                if (formatter == null)
                    throw new NotImplementedException($"Formatter for type {format.Type} not registered.");

                localFormat.Model = formatterDelegate(formatter, player, format.Model);
                localFormat.Type  = localFormat.Model.Type;

                yield return localFormat;
            }
        }
    }
}
