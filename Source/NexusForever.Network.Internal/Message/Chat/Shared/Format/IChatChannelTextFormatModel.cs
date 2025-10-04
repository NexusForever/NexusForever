using System.Text.Json.Serialization;
using NexusForever.Game.Static.Social;
using NexusForever.Network.Internal.Message.Chat.Shared.Format.Model;

namespace NexusForever.Network.Internal.Message.Chat.Shared.Format
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(ChatChannelTextItemIdFormat), nameof(ChatFormatType.ItemId))]
    [JsonDerivedType(typeof(ChatChannelTextQuestIdFormat), nameof(ChatFormatType.QuestId))]
    public interface IChatChannelTextFormatModel
    {
        ChatFormatType Type { get; }
    }
}
