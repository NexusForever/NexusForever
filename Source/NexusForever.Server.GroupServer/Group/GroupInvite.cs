using NexusForever.Database.Group.Model;

namespace NexusForever.Server.GroupServer.Group
{
    public class GroupInvite : IWrappedModel<GroupInviteModel>
    {
        public GroupInviteModel Model { get; private set; }

        public Identity Inviter
        {
            get => new Identity()
            {
                Id      = Model.InviterCharacterId,
                RealmId = Model.InviterRealmId
            };
            set
            {
                Model.InviterCharacterId = value.Id;
                Model.InviterRealmId     = value.RealmId;
            }
        }

        public Identity Invitee
        {
            get => new Identity()
            {
                Id      = Model.InviteeCharacterId,
                RealmId = Model.InviteeRealmId
            };
            set
            {
                Model.InviteeCharacterId = value.Id;
                Model.InviteeRealmId     = value.RealmId;
            }
        }

        public DateTime Expiration
        {
            get => Model.Expiration;
            set => Model.Expiration = value;
        }

        /// <summary>
        /// Initialise a new group invite.
        /// </summary>
        public void Initialise()
        {
            if (Model != null)
                throw new InvalidOperationException("GroupInvite has already been initialised.");

            Model = new GroupInviteModel();
        }

        /// <summary>
        /// Initialise group invite with model.
        /// </summary>
        /// <param name="model">Model to initialise the group invite.</param>
        public void Initialise(GroupInviteModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("GroupInvite has already been initialised.");

            Model = model;
        }
    }
}
