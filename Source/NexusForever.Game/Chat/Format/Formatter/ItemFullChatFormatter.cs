using NexusForever.Game.Abstract.Chat.Format;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat.Shared.Format;
using NexusForever.Network.Internal.Message.Chat.Shared.Format.Model;
using NexusForever.Network.World.Chat;
using NexusForever.Network.World.Chat.Model;

namespace NexusForever.Game.Chat.Format.Formatter
{
    public class ItemFullChatFormatter : IInternalChatFormatter<ChatFormatItemFull>, INetworkChatFormatter<ChatChannelTextItemFullFormat>
    {
        public IChatChannelTextFormatModel ToInternal(IPlayer player, ChatFormatItemFull format)
        {
            return new ChatChannelTextItemFullFormat
            {
                ItemGuid = format.ItemGuid,
                Item2Id = format.Item2Id,
                MakerCharacterId = format.MakerCharacterId,
                CircuitData = format.CircuitData,
                RuneData = format.RuneData, 
                ThresholdData = format.ThresholdData,
                Unknown1 = format.Unknown1,
                WorldRequirements = format.WorldRequirements,
                Unknown2 = format.Unknown2,
                WorldRequirementItem2Id = format.WorldRequirementItem2Id,
                ChargeAmounts = format.ChargeAmounts,
                RuneSlotItem2Ids = format.RuneSlotItem2Ids,
            };
        }

        public IChatFormatModel ToNetwork(ChatChannelTextItemFullFormat format)
        {
            return new ChatFormatItemFull
            {
                ItemGuid = format.ItemGuid,
                Item2Id = format.Item2Id,
                MakerCharacterId = format.MakerCharacterId,
                CircuitData = format.CircuitData,
                RuneData = format.RuneData,
                ThresholdData = format.ThresholdData,
                Unknown1 = format.Unknown1,
                WorldRequirements = format.WorldRequirements,
                Unknown2 = format.Unknown2,
                WorldRequirementItem2Id = format.WorldRequirementItem2Id,
                ChargeAmounts = format.ChargeAmounts,
                RuneSlotItem2Ids = format.RuneSlotItem2Ids,
            };
        }
    }
}
