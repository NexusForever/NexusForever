using Microsoft.Extensions.DependencyInjection;
using NexusForever.Database.Group.Model;
using NexusForever.Database.Group.Repository;
using NexusForever.Game.Static.Group;
using NexusForever.Game.Static.Matching;

namespace NexusForever.Server.GroupServer.Group;

public class GroupManager
{
    private readonly Dictionary<ulong, Group> _scopedGroups = [];

    #region Dependency Injection

    private readonly IServiceProvider _serviceProvider;
    private readonly GroupRepository _groupRepository;

    public GroupManager(
        IServiceProvider serviceProvider,
        GroupRepository groupRepository)
    {
        _serviceProvider = serviceProvider;
        _groupRepository = groupRepository;
    }

    #endregion

    /// <summary>
    /// Create a new open world group.
    /// </summary>
    public Group CreateOpenWorldGroup()
    {
        return CreateGroup(GroupFlags.OpenWorld);
    }

    /// <summary>
    /// Create a new instance group.
    /// </summary>
    public Group CreateInstanceGroup()
    {
        return CreateGroup(GroupFlags.None);
    }

    private Group CreateGroup(GroupFlags flags)
    {
        Group group = _serviceProvider.GetRequiredService<Group>();
        group.Initialise();
        group.Flags = flags;
        _groupRepository.AddGroup(group.Model);

        return group;
    }

    /// <summary>
    /// Remove a group.
    /// </summary>
    /// <param name="group">Group to remove.</param>
    public void RemoveGroup(Group group)
    {
        _scopedGroups.Remove(group.Id);
        _groupRepository.RemoveGroup(group.Model);
    }

    /// <summary>
    /// Get a group.
    /// </summary>
    /// <param name="id">Id of group to return.</param>
    public async Task<Group> GetGroupAsync(ulong id)
    {
        if (_scopedGroups.TryGetValue(id, out Group group))
            return group;

        GroupModel model = await _groupRepository.GetGroupAsync(id);
        if (model == null)
            return null;

        group = _serviceProvider.GetRequiredService<Group>();
        group.Initialise(model);
        _scopedGroups.Add(model.GroupId, group);

        return group;
    }

    /// <summary>
    /// Get an instance group.
    /// </summary>
    /// <param name="match">Guid of the match the group belongs to.</param>
    /// <param name="matchTeam">Team id in the match the group belongs to.</param>
    public async Task<Group> GetGroupAsync(Guid match, MatchTeam matchTeam)
    {
        GroupModel model = await _groupRepository.GetGroupByMatchAsync(match, matchTeam);
        if (model == null)
            return null;

        Group group = _serviceProvider.GetRequiredService<Group>();
        group.Initialise(model);
        _scopedGroups.Add(model.GroupId, group);
        return group;
    }
}
