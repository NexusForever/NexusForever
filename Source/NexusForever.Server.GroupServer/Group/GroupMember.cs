using NexusForever.Database.Group.Model;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Server.GroupServer.Character;
using NexusForever.Server.GroupServer.Network.Internal;

namespace NexusForever.Server.GroupServer.Group
{
    public class GroupMember : IWrappedModel<GroupMemberModel>
    {
        public GroupMemberModel Model { get; private set; }

        public Group Group
        {
            get => _group;
            private set
            {
                _group        = value;
                Model.Group   = _group.Model;
                Model.GroupId = _group.Id;
            }
        }

        private Group _group;

        public Identity Identity
        {
            get => new Identity
            {
                Id      = Model.CharacterId,
                RealmId = Model.RealmId
            };
            set
            {
                Model.CharacterId = value.Id;
                Model.RealmId     = value.RealmId;
            }
        }

        public uint Index
        {
            get => Model.Index;
            set => Model.Index = value;
        }

        public GroupMemberInfoFlags Flags
        {
            get => Model.Flags;
            set => Model.Flags = value;
        }

        public bool PositionDirty
        {
            get => Model.PositionDirty;
            set => Model.PositionDirty = value;
        }

        #region Dependency Injection

        private readonly CharacterManager _characterManager;
        private readonly IInternalMessagePublisher _messagePublisher;

        public GroupMember(
            CharacterManager characterManager,
            OutboxMessagePublisher messagePublisher)
        {
            _characterManager = characterManager;
            _messagePublisher = messagePublisher;
        }

        #endregion


        /// <summary>
        /// Initialise a new group member.
        /// </summary>
        /// <param name="group">Group that owns the group member.</param>
        public void Initialise(Group group)
        {
            if (Model != null)
                throw new InvalidOperationException("GroupMember already initialised.");

            Model = new GroupMemberModel();
            Group = group;
        }

        /// <summary>
        /// Initialise a group member with model.
        /// </summary>
        /// <param name="group">Group that owns the group member.</param>
        /// <param name="model">Model to initialise the group member.</param>
        public void Initialise(Group group, GroupMemberModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("GroupMember already initialised.");

            Model = model;
            Group = group;
        }

        /// <summary>
        /// Get the character associated with this group member.
        /// </summary>
        public async Task<Character.Character> GetCharacterAsync()
        {
            return await _characterManager.GetCharacterRemoteAsync(Identity);
        }

        /// <summary>
        /// Check if group member flags can be set.
        /// </summary>
        /// <param name="updater">Identity of the group member updating the flags.</param>
        /// <param name="flags">Flags to either set or unset.</param>
        public bool CanSetFlags(Identity updater, GroupMemberInfoFlags flags)
        {
            // If we are role locked and we are not the leader, we cannot update the flags.
            if (Flags.HasFlag(GroupMemberInfoFlags.RoleLocked))
                return false;

            if ((Flags & GroupMemberInfoFlags.RaidAssistant) != 0)
                return true;

            if (Identity != updater)
                return false;

            GroupMemberInfoFlags allowedFlags = GroupMemberInfoFlags.RoleFlags
                | GroupMemberInfoFlags.HasSetReady
                | GroupMemberInfoFlags.Ready;
            return (flags & allowedFlags) == flags;
        }

        /// <summary>
        /// Set group member flags.
        /// </summary>
        /// <param name="flags">Flags to set.</param>
        public async Task SetFlagAsync(GroupMemberInfoFlags flags, bool fromPromotion = false)
        {
            Flags |= flags;

            await _messagePublisher.PublishAsync(new GroupMemberFlagsUpdatedMessage
            {
                Group         = await Group.ToInternalGroup(),
                Member        = await this.ToInternalGroupMember(),
                FromPromotion = fromPromotion
            });
        }

        /// <summary>
        /// Remove group member flags.
        /// </summary>
        /// <param name="flags">Flags to unset.</param>
        public async Task RemoveFlagAsync(GroupMemberInfoFlags flags, bool fromPromotion = false)
        {
            Flags &= ~flags;

            await _messagePublisher.PublishAsync(new GroupMemberFlagsUpdatedMessage
            {
                Group         = await Group.ToInternalGroup(),
                Member        = await this.ToInternalGroupMember(),
                FromPromotion = fromPromotion
            });
        }
    }
}
