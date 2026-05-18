using NexusForever.Database.Group.Model;
using NexusForever.Game.Static.Group;

namespace NexusForever.Server.GroupServer.Group
{
    public class GroupRequest : IWrappedModel<GroupRequestModel>
    {
        public GroupRequestModel Model { get; private set; }

        public Identity RequesterIdentity
        {
            get => new Identity
            {
                Id      = Model.RequesterCharacterId,
                RealmId = Model.RequesterRealmId
            };
            set
            {
                Model.RequesterCharacterId = value.Id;
                Model.RequesterRealmId = value.RealmId;
            }
        }

        public Identity RequesteeIdentity
        {
            get => new Identity
            {
                Id      = Model.RequesteeCharacterId,
                RealmId = Model.RequesteeRealmId
            };
            set
            {
                Model.RequesteeCharacterId = value.Id;
                Model.RequesteeRealmId = value.RealmId;
            }
        }

        public GroupRequestType Type
        {
            get => Model.RequestType;
            set => Model.RequestType = value;
        }

        public DateTime Expiration
        {
            get => Model.Expiration;
            set => Model.Expiration = value;
        }

        /// <summary>
        /// Initialise a new group request.
        /// </summary>
        public void Initialise()
        {
            if (Model != null)
                throw new InvalidOperationException("GroupRequest has already been initialised.");

            Model = new GroupRequestModel();
        }

        /// <summary>
        /// Initialise group request with model.
        /// </summary>
        /// <param name="model">Model to initialise the group request.</param>
        public void Initialise(GroupRequestModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("GroupRequest has already been initialised.");

            Model = model;
        }
    }
}
