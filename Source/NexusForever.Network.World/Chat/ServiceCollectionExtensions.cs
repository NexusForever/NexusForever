using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Static.Social;
using NexusForever.Network.World.Chat.Model;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Chat
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNetworkWorldChat(this IServiceCollection sc)
        {
            sc.AddTransient<IChatFormatModelFactory, ChatFormatModelFactory>();

            sc.AddTransient<ChatClientFormat>();
            sc.AddKeyedTransient<IChatFormatModel, ChatFormatItemGuid>(ChatFormatType.ItemGuid);
            sc.AddKeyedTransient<IChatFormatModel, ChatFormatItemId>(ChatFormatType.ItemId);
            sc.AddKeyedTransient<IChatFormatModel, ChatFormatQuestId>(ChatFormatType.QuestId);
        }
    }
}
