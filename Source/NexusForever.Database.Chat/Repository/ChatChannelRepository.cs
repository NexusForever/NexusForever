using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Chat.Model;
using NexusForever.Game.Static.Chat;

namespace NexusForever.Database.Chat.Repository
{
    public class ChatChannelRepository
    {
        private readonly ChatContext _context;

        public ChatChannelRepository(
            ChatContext context)
        {
            _context = context;
        }

        public void AddChatChannel(ChatChannelModel chatChannel)
        {
            _context.ChatChannel.Add(chatChannel);
        }

        public void RemoveChatChannel(ChatChannelModel chatChannel)
        {
            _context.ChatChannel.Remove(chatChannel);
        }

        public async Task<ChatChannelModel> GetChatChannelAsync(ulong chatId)
        {
            return await IncludeChatChannel(_context.ChatChannel)
                .FirstOrDefaultAsync(c => c.ChatId == chatId);
        }

        public async Task<ChatChannelModel> GetChatChannelAsync(ChatChannelType type, ChatChannelReferenceType referenceType, ulong referenceValue)
        {
            return await IncludeChatChannel(_context.ChatChannel)
                .FirstOrDefaultAsync(c => c.Type == type && c.ReferenceType == referenceType && c.ReferenceValue == referenceValue);
        }

        public async Task<List<ChatChannelModel>> GetChatChannelsAsync(ChatChannelReferenceType referenceType, ulong referenceValue)
        {
            return await IncludeChatChannel(_context.ChatChannel)
                .Where(c => c.ReferenceType == referenceType && c.ReferenceValue == referenceValue)
                .ToListAsync();
        }

        public async Task<ChatChannelModel> GetChatChannelAsync(ChatChannelType type, string name)
        {
            return await IncludeChatChannel(_context.ChatChannel)
                .FirstOrDefaultAsync(c => c.Type == type && c.Name == name);
        }

        private IQueryable<ChatChannelModel> IncludeChatChannel(IQueryable<ChatChannelModel> query)
        {
            return query
                .Include(c => c.Owner)
                .Include(c => c.Members);
        }
    }
}
