using NexusForever.Database;
using NexusForever.Database.Friendship.Model;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Server.Friendship.Game.Character
{
    public class CharacterStat : IWrappedModel<CharacterStatModel>
    {
        public CharacterStatModel Model { get; private set; }

        public Stat Stat => Model.Stat;

        public float Value
        {
            get => Model.Value;
            set => Model.Value = value;
        }

        /// <summary>
        /// Initialise a new character stat.
        /// </summary>
        public void Initialise(Stat stat, float value)
        {
            if (Model != null)
                throw new InvalidOperationException("CharacterStat is already initialised.");

            Model = new CharacterStatModel
            {
                Stat  = stat,
                Value = value
            };
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
