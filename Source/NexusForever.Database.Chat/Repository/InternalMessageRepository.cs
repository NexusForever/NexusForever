using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Chat.Model;

namespace NexusForever.Database.Chat.Repository
{
    public class InternalMessageRepository
    {
        #region Dependency Injection

        private readonly ChatContext _context;

        public InternalMessageRepository(
            ChatContext context)
        {
            _context = context;
        }

        #endregion

        public void AddMessage(InternalMessageModel message)
        {
            _context.InternalMessage.Add(message);
        }

        public void RemoveMessage(InternalMessageModel message)
        {
            _context.InternalMessage.Remove(message);
        }

        public async Task<InternalMessageModel> GetNextMessageAsync()
        {
            return await _context.InternalMessage
                .Where(m => m.ProcessedAt == null)
                .OrderBy(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<InternalMessageModel> GetMessageAsync(Guid id)
        {
            return await _context.InternalMessage
                .Where(m => m.Id == id && m.ProcessedAt == null)
                .FirstOrDefaultAsync();
        }
    }
}
