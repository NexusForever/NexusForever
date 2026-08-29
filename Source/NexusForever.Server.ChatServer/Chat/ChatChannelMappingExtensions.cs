using NexusForever.Server.ChatServer;
using NexusForever.Server.ChatServer.Character;
using NexusForever.Server.ChatServer.Chat;
using InternalChatChannel = NexusForever.Network.Internal.Message.Chat.Shared.ChatChannel;
using InternalChatChannelMember = NexusForever.Network.Internal.Message.Chat.Shared.ChatChannelMember;

namespace NexusForever.Network.Internal.Message.Chat
{
    public static class ChatChannelMappingExtensions
    {
        public static async Task<InternalChatChannel> ToInternalChatChannelAsync(this ChatChannel chatChannel)
        {
            var members = new List<InternalChatChannelMember>();
            foreach (ChatChannelMember chatChannelMember in chatChannel.GetMembers())
                members.Add(await chatChannelMember.ToInternalChatChannelMemberAsync());

            return new InternalChatChannel
            {
                ChatId         = chatChannel.ChatId,
                Type           = chatChannel.Type,
                Name           = chatChannel.Name,
                Password       = chatChannel.Password,
                Members        = members,
                ReferenceType  = chatChannel.ReferenceType,
                ReferenceValue = chatChannel.ReferenceValue,
            };
        }

        public static async Task<InternalChatChannelMember> ToInternalChatChannelMemberAsync(this ChatChannelMember chatChannelMember)
        {
            return new InternalChatChannelMember
            {
                Identity  = chatChannelMember.Identity.ToInternalIdentity(),
                Flags     = chatChannelMember.Flags,
                Character = (await chatChannelMember.GetCharacterAsync()).ToInternalCharacter()
            };
        }
    }
}
