using System.Text.Json.Serialization;
using NexusForever.Game.Static.Chat;
using NexusForever.Network.Internal.Message.Chat.Shared.Format.Model;

namespace NexusForever.Network.Internal.Message.Chat.Shared.Format
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(ChatChannelTextFormat0Format), nameof(ChatFormatType.Format0))]
    [JsonDerivedType(typeof(ChatChannelTextAlienFormat), nameof(ChatFormatType.Alien))]
    [JsonDerivedType(typeof(ChatChannelTextRoleplayFormat), nameof(ChatFormatType.Roleplay))]
    [JsonDerivedType(typeof(ChatChannelTextFormat3Format), nameof(ChatFormatType.Format3))]
    [JsonDerivedType(typeof(ChatChannelTextItemIdFormat), nameof(ChatFormatType.ItemId))]
    [JsonDerivedType(typeof(ChatChannelTextQuestIdFormat), nameof(ChatFormatType.QuestId))]
    [JsonDerivedType(typeof(ChatChannelTextArchiveArticleFormat), nameof(ChatFormatType.ArchiveArticle))]
    [JsonDerivedType(typeof(ChatChannelTextProfanityFormat), nameof(ChatFormatType.Profanity))]
    [JsonDerivedType(typeof(ChatChannelTextItemFullFormat), nameof(ChatFormatType.ItemFull))]
    [JsonDerivedType(typeof(ChatChannelTextItemGuidFormat), nameof(ChatFormatType.ItemGuid))]
    [JsonDerivedType(typeof(ChatChannelTextNavPointFormat), nameof(ChatFormatType.NavPoint))]
    [JsonDerivedType(typeof(ChatChannelTextLootFormat), nameof(ChatFormatType.Loot))]
        
    public interface IChatChannelTextFormatModel
    {
        ChatFormatType Type { get; }
    }
}
