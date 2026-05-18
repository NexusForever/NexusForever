using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Friendship.Model;

namespace NexusForever.Database.Friendship.Repository
{
    public class InternalMessageRepository
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;

        public InternalMessageRepository(
            FriendshipContext context)
        {
            _context = context;
        }

        #endregion

        public void AddMessage(InternalMessageModel message)
        {
            _context.InternalMessage.Add(message);
        }

        public async Task<InternalMessageModel> GetNextMessageAsync()
        {
            return await _context.InternalMessage
                .Where(m => m.ProcessedAt == null)
                .OrderBy(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
