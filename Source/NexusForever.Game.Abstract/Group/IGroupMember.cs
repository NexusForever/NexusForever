using InternalGroupMember = NexusForever.Network.Internal.Message.Group.Shared.GroupMember;

namespace NexusForever.Game.Abstract.Group
{
    public interface IGroupMember
    {
        Identity Identity { get; }

        void Initialise(InternalGroupMember internalGroupMember);
    }
}
