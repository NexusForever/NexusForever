using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Static.Chat;
using NexusForever.Network.World.Chat.Model;
using NexusForever.Network.World.Message.Model.Chat;

namespace NexusForever.Network.World.Chat
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNetworkWorldChat(this IServiceCollection sc)
        {
            sc.AddTransient<IChatFormatModelFactory, ChatFormatModelFactory>();

            sc.AddTransient<ChatClientFormat>();
            sc.AddKeyedTransient<IChatFormatModel, ChatFormat0>(ChatFormatType.Format0);
            sc.AddKeyedTransient<IChatFormatModel, ChatFormatAlien>(ChatFormatType.Alien);
            sc.AddKeyedTransient<IChatFormatModel, ChatFormatRoleplay>(ChatFormatType.Roleplay);
            sc.AddKeyedTransient<IChatFormatModel, ChatFormat3>(ChatFormatType.Format3);
            sc.AddKeyedTransient<IChatFormatModel, ChatFormatItemId>(ChatFormatType.ItemId);
            sc.AddKeyedTransient<IChatFormatModel, ChatFormatQuestId>(ChatFormatType.QuestId);
            sc.AddKeyedTransient<IChatFormatModel, ChatFormatArchiveArticle>(ChatFormatType.ArchiveArticle);
            sc.AddKeyedTransient<IChatFormatModel, ChatFormatProfanity>(ChatFormatType.Profanity);
            sc.AddKeyedTransient<IChatFormatModel, ChatFormatItemFull>(ChatFormatType.ItemFull);
            sc.AddKeyedTransient<IChatFormatModel, ChatFormatItemGuid>(ChatFormatType.ItemGuid);
            sc.AddKeyedTransient<IChatFormatModel, ChatFormatNavPoint>(ChatFormatType.NavPoint);
            sc.AddKeyedTransient<IChatFormatModel, ChatFormatLoot>(ChatFormatType.Loot);
        }
    }
}
