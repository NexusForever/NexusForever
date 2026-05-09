using NexusForever.Game.Abstract.Chat.Format;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat.Shared.Format;
using NexusForever.Network.Internal.Message.Chat.Shared.Format.Model;
using NexusForever.Network.World.Chat;
using NexusForever.Network.World.Chat.Model;

namespace NexusForever.Game.Chat.Format.Formatter
{
    public class LootChatFormatter : IInternalChatFormatter<ChatFormatLoot>, INetworkChatFormatter<ChatChannelTextLootFormat>
    {
        public IChatChannelTextFormatModel ToInternal(IPlayer player, ChatFormatLoot format)
        {
            return new ChatChannelTextLootFormat
            {
                LootUnitId = format.LootUnitId,
            };
        }

        public IChatFormatModel ToNetwork(ChatChannelTextLootFormat format)
        {
            return new ChatFormatLoot
            {
                LootUnitId = format.LootUnitId,
            };
        }
    }
}
