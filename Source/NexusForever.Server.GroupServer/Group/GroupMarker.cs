using NexusForever.Database.Group.Model;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Server.GroupServer.Network.Internal;

namespace NexusForever.Server.GroupServer.Group
{
    public class GroupMarker : IWrappedModel<GroupMarkerModel>
    {
        public GroupMarkerModel Model { get; private set; }

        public Group Group
        {
            get => _group;
            private set
            {
                _group        = value;
                Model.Group   = _group?.Model;
                Model.GroupId = _group?.Id ?? 0;
            }
        }

        private Group _group;

        public Game.Static.Group.GroupMarker Marker
        {
            get => Model.Marker;
            set => Model.Marker = value;
        }

        public uint UnitId
        {
            get => Model.UnitId;
            set => Model.UnitId = value;
        }

        #region Dependency Injection

        private readonly IInternalMessagePublisher _messagePublisher;

        public GroupMarker(
            OutboxMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        #endregion

        /// <summary>
        /// Initialise a new group marker.
        /// </summary>
        /// <param name="group">Group that owns the group marker.</param>
        public void Initialise(Group group)
        {
            if (Model != null)
                throw new InvalidOperationException("GroupMarker has already been initialised.");

            Model = new GroupMarkerModel();
            Group = group;
        }

        /// <summary>
        /// Initialise a group marker with model.
        /// </summary>
        /// <param name="group">Group that owns the group marker.</param>
        /// <param name="model">Model to initialise the group marker.</param>
        public void Initialise(Group group, GroupMarkerModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("GroupMarker has already been initialised.");

            Model = model;
            Group = group;
        }

        /// <summary>
        /// Set the unit marker.
        /// </summary>
        /// <param name="unitId">Unit id to assign the marker to.</param>
        public async Task SetMarkerAsync(uint? unitId)
        {
            UnitId = unitId ?? 0;

            await _messagePublisher.PublishAsync(new GroupMarkerUpdatedMessage
            {
                Group  = await Group.ToInternalGroup(),
                Marker = Marker,
                UnitId = unitId
            });
        }
    }
}
