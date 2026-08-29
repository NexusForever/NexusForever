using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Chat.Format;
using NexusForever.Game.Chat.Format.Formatter;
using NexusForever.Network.Internal.Message.Chat.Shared.Format.Model;
using NexusForever.Network.World.Chat.Model;

namespace NexusForever.Game.Chat.Format
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameChatFormat(this IServiceCollection sc)
        {
            sc.AddSingleton<IChatFormatManager, ChatFormatManager>();

            sc.AddTransient<IInternalChatFormatter<ChatFormatItemGuid>, ItemGuidChatFormatter>();
            sc.AddTransient<IInternalChatFormatter<ChatFormatItemId>, ItemIdChatFormatter>();
            sc.AddTransient<IInternalChatFormatter<ChatFormatQuestId>, QuestIdChatFormatter>();

            sc.AddTransient<INetworkChatFormatter<ChatChannelTextItemIdFormat>, ItemIdChatFormatter>();
            sc.AddTransient<INetworkChatFormatter<ChatChannelTextQuestIdFormat>, QuestIdChatFormatter>();

            sc.AddTransient<ILocalChatFormatter<ChatFormatItemGuid>, ItemGuidChatFormatter>();
        }
    }
}
