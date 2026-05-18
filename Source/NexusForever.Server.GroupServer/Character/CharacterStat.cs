using NexusForever.Database.Group.Model;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Server.GroupServer.Character
{
    public class CharacterStat : IWrappedModel<CharacterStatModel>
    {
        public CharacterStatModel Model { get; private set; }

        public Stat Stat
        {
            get => Model.Stat;
            set => Model.Stat = value;
        }

        public float Value
        {
            get => Model.Value;
            set => Model.Value = value;
        }

        /// <summary>
        /// Initialise a new character stat.
        /// </summary>
        public void Initialise()
        {
            if (Model != null)
                throw new InvalidOperationException("CharacterStat is already initialised.");

            Model = new CharacterStatModel();
        }

        /// <summary>
        /// Initialise character stat with model.
        /// </summary>
        /// <param name="model">Model to initialise character stat.</param>
        public void Initialise(CharacterStatModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("CharacterStat is already initialised.");

            Model = model;
        }
    }
}
