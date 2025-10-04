using NexusForever.Database.Group.Model;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Server.GroupServer.Character
{
    public class CharacterProperty : IWrappedModel<CharacterPropertyModel>
    {
        public CharacterPropertyModel Model { get; private set; }

        public Property Property
        {
            get => Model.Property;
            set => Model.Property = value;
        }

        public float Value
        {
            get => Model.Value;
            set => Model.Value = value;
        }

        /// <summary>
        /// Initialise a new character property.
        /// </summary>
        public void Initialise()
        {
            if (Model != null)
                throw new InvalidOperationException("CharacterProperty is already initialised.");

            Model = new CharacterPropertyModel();
        }

        /// <summary>
        /// Initialise character property with model.
        /// </summary>
        /// <param name="model">Model to initialise character property.</param>
        public void Initialise(CharacterPropertyModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("CharacterProperty is already initialised.");

            Model = model;
        }
    }
}
