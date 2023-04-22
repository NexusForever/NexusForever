using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Entity
{
    public class StatValue : IStatValue
    {
        [Flags]
        public enum StatSaveMask
        {
            None   = 0x00,
            Create = 0x01,
            Value  = 0x02
        }

        public Stat Stat { get; }
        public StatType Type { get; }

        public float Value
        {
            get => value;
            set
            {
                this.value = value;
                saveMask |= StatSaveMask.Value;
            }
        }

        private float value;

        public uint Data { get; set; }

        private StatSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="IStatValue"/> from an existing database model.
        /// </summary>
        public StatValue(CharacterStatModel model)
        {
            Stat  = (Stat)model.Stat;
            Type  = EntityManager.Instance.GetStatAttribute(Stat).Type;
            Value = model.Value;
        }

        /// <summary>
        /// Create a new <see cref="IStatValue"/> from an existing database model.
        /// </summary>
        public StatValue(EntityStatModel model)
        {
            Stat  = (Stat)model.Stat;
            Type  = EntityManager.Instance.GetStatAttribute(Stat).Type;
            Value = model.Value;
        }

        /// <summary>
        /// Create a new <see cref="IStatValue"/> from supplied <see cref="Stat"/> and value.
        /// </summary>
        public StatValue(Stat stat, uint value)
        {
            Stat     = stat;
            Type     = StatType.Integer;
            Value    = value;
            saveMask = StatSaveMask.Create;
        }

        /// <summary>
        /// Create a new <see cref="IStatValue"/> from supplied <see cref="Stat"/> and value.
        /// </summary>
        public StatValue(Stat stat, float value)
        {
            Stat     = stat;
            Type     = StatType.Float;
            Value    = value;
            saveMask = StatSaveMask.Create;
        }

        public void SaveCharacter(ulong characterId, CharacterContext context)
        {
            if (saveMask == StatSaveMask.None)
                return;

            if ((saveMask & StatSaveMask.Create) != 0)
            {
                context.Add(new CharacterStatModel
                {
                    Id    = characterId,
                    Stat  = (byte)Stat,
                    Value = Value
                });
            }
            else
            {
                var statModel = new CharacterStatModel
                {
                    Id   = characterId,
                    Stat = (byte)Stat
                };

                EntityEntry<CharacterStatModel> statEntity = context.Attach(statModel);
                if ((saveMask & StatSaveMask.Value) != 0)
                {
                    statModel.Value = Value;
                    statEntity.Property(p => p.Value).IsModified = true;
                }
            }

            saveMask = StatSaveMask.None;
        }

        /*public void SaveEntity(CharacterContext context)
        {
        }*/
    }
}
