using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Group.Model;

namespace NexusForever.Database.Group.Repository
{
    public class InternalMessageRepository
    {
        #region Dependency Injection

        private readonly GroupContext _context;

        public InternalMessageRepository(
            GroupContext context)
        {
            _context = context;
        }

        #endregion

        public void AddMessage(InternalMessageModel message)
        {
            _context.InternalMessage.Add(message);
        }

        public async Task<InternalMessageModel> GetNextMessage()
        {
            return await _context.InternalMessage
                .Where(m => m.ProcessedAt == null)
                .OrderBy(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
