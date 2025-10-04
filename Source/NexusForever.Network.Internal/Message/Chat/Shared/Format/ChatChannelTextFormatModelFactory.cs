using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Static.Social;

namespace NexusForever.Network.Internal.Message.Chat.Shared.Format
{
    public class ChatChannelTextFormatModelFactory
    {
        #region Dependency Injection

        private readonly IServiceProvider serviceProvider;

        public ChatChannelTextFormatModelFactory(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        #endregion

        public IChatChannelTextFormatModel NewChatFormatModel(ChatFormatType type)
        {
            return serviceProvider.GetKeyedService<IChatChannelTextFormatModel>(type);
        }
    }
}
