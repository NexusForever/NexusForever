using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Database.World.Model;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity
{
    public class StatValue
    {
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
        /// Create a new <see cref="StatValue"/> from an existing database model.
        /// </summary>
        public StatValue(CharacterStatModel model)
        {
            Stat  = (Stat)model.Stat;
            Type  = EntityManager.Instance.GetStatAttribute(Stat).Type;
            Value = model.Value;
        }

        /// <summary>
        /// Create a new <see cref="StatValue"/> from an existing database model.
        /// </summary>
        public StatValue(EntityStatModel model)
        {
            Stat  = (Stat)model.Stat;
            Type  = EntityManager.Instance.GetStatAttribute(Stat).Type;
            Value = model.Value;
        }

        /// <summary>
        /// Create a new <see cref="StatValue"/> from supplied <see cref="Stat"/> and value.
        /// </summary>
        public StatValue(Stat stat, uint value)
        {
            Stat     = stat;
            Type     = StatType.Integer;
            Value    = value;
            saveMask = StatSaveMask.Create;
        }

        /// <summary>
        /// Create a new <see cref="StatValue"/> from supplied <see cref="Stat"/> and value.
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

        public void WriteInitial(GamePacketWriter writer)
        {
            writer.Write(Stat, 5u);
            writer.Write(Type, 2u);

            switch (Type)
            {
                case StatType.Integer:
                    writer.Write((uint)Value);
                    break;
                case StatType.Float:
                    writer.Write(Value);
                    break;
                case StatType.Data:
                    writer.Write((uint)Value);
                    writer.Write(Data);
                    break;
            }
        }

        public void WriteUpdate(GamePacketWriter writer)
        {
            writer.Write(Stat, 5u);

            switch (Type)
            {
                case StatType.Integer:
                    writer.Write((uint)Value);
                    break;
                case StatType.Float:
                    writer.Write(Value);
                    break;
            }
        }
    }
}
