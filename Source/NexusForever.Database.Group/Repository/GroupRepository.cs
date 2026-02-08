using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Group.Model;
using NexusForever.Game.Static.Matching;

namespace NexusForever.Database.Group.Repository
{
    public class GroupRepository
    {
        #region Dependency Injection

        private readonly GroupContext _context;

        public GroupRepository(
            GroupContext context)
        {
            _context = context;
        }

        #endregion

        public void AddGroup(GroupModel model)
        {
            _context.Group.Add(model);
        }

        public void RemoveGroup(GroupModel model)
        {
            _context.Group.Remove(model);
        }

        public async Task<GroupModel> GetGroupAsync(ulong groupId)
        {
            return await IncludeGroup(_context.Group)
                .FirstOrDefaultAsync(g => g.GroupId == groupId);
        }

        public async Task<GroupModel> GetGroupByMatchAsync(Guid match, MatchTeam matchTeam)
        {
            return await IncludeGroup(_context.Group)
                .FirstOrDefaultAsync(g => g.Match == match && g.MatchTeam == matchTeam);
        }

        public async Task<GroupModel> GetNextGroupWithExpiredRequest()
        {
            return await IncludeGroup(_context.Group)
                .FirstOrDefaultAsync(g => g.Request != null && g.Request.Expiration <= DateTime.UtcNow);
        }

        public async Task<GroupModel> GetNextGroupWithExpiredInvite()
        {
            return await IncludeGroup(_context.Group)
                .FirstOrDefaultAsync(g => g.Invites.Any(i => i.Expiration <= DateTime.UtcNow));
        }

        public async Task<GroupModel> GetNextGroupWithDirtyPosition()
        {
            return await IncludeGroup(_context.Group)
                .FirstOrDefaultAsync(g => g.Members.Any(m => m.PositionDirty));
        }

        private IQueryable<GroupModel> IncludeGroup(IQueryable<GroupModel> query)
        {
            return query
                .AsSplitQuery()
                .Include(g => g.Leader)
                .Include(g => g.Request)
                .Include(g => g.Markers)
                .Include(g => g.Members)
                .Include(g => g.Invites);
        }
    }
}
