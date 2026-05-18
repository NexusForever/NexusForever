using NexusForever.Database.Group.Model;
using NexusForever.Server.GroupServer.Group;

namespace NexusForever.Server.GroupServer.Character
{
    public class CharacterGroup : IWrappedModel<CharacterGroupModel>
    {
        public CharacterGroupModel Model { get; private set; }

        public uint Index
        {
            get => Model.Index;
            set => Model.Index = value;
        }

        public ulong GroupId
        {
            get => Model.GroupId;
            set => Model.GroupId = value;
        }

        #region Dependency Injection

        private readonly GroupManager _groupManager;

        public CharacterGroup(
            GroupManager groupManager)
        {
            _groupManager = groupManager;
        }

        #endregion

        /// <summary>
        /// Initialise a new character group.
        /// </summary>
        public void Initialise()
        {
            if (Model != null)
                throw new InvalidOperationException("CharacterGroup has already been initialised.");

            Model = new CharacterGroupModel();
        }

        /// <summary>
        /// Initialise character group with model.
        /// </summary>
        /// <param name="model">Model to initialise the character group.</param>
        public void Initialise(CharacterGroupModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("CharacterGroup has already been initialised.");

            Model = model;
        }

        /// <summary>
        /// Get the group associated with this character group.
        /// </summary>
        public async Task<Group.Group> GetGroupAsync()
        {
            return await _groupManager.GetGroupAsync(Model.GroupId);
        }
    }
}
