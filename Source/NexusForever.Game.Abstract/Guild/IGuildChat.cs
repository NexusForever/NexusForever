using NexusForever.Game.Static.Social;

namespace NexusForever.Game.Abstract.Guild
{
    public interface IGuildChat : IGuildBase
    {
        void InitialiseChatChannels(ChatChannelType? memberChannelType, ChatChannelType? officerChannelType);
    }
}