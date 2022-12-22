using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Entity
{
    public class Datacube
    {
        [Flags]
        public enum DatacubeSaveMask
        {
            None     = 0x0000,
            Create   = 0x0001,
            Progress = 0x0002
        }

        public ushort Id { get; }
        public DatacubeType Type { get; }

        public uint Progress
        {
            get => progress;
            set
            {
                progress = value;
                saveMask |= DatacubeSaveMask.Progress;
            }
        }

        private uint progress;

        private DatacubeSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="Datacube"/> from the supplied id, <see cref="DatacubeType"/> and progress.
        /// </summary>
        public Datacube(ushort id, DatacubeType type, uint progress)
        {
            Id       = id;
            Type     = type;
            Progress = progress;

            saveMask = DatacubeSaveMask.Create;
        }

        /// <summary>
        /// Create a new <see cref="Datacube"/> from an existing database model.
        /// </summary>
        public Datacube(CharacterDatacubeModel model)
        {
            Id       = model.Datacube;
            Type     = (DatacubeType)model.Type;
            Progress = model.Progress;
        }

        public void Save(CharacterContext context, ulong characterId)
        {
            if (saveMask == DatacubeSaveMask.None)
                return;

            var model = new CharacterDatacubeModel
            {
                Id       = characterId,
                Type     = (byte)Type,
                Datacube = Id,
                Progress = Progress
            };

            if ((saveMask & DatacubeSaveMask.Create) != 0)
                context.Add(model);
            else if ((saveMask & DatacubeSaveMask.Progress) != 0)
            {
                EntityEntry<CharacterDatacubeModel> entity = context.Attach(model);
                entity.Property(p => p.Progress).IsModified = true;
            }

            saveMask = DatacubeSaveMask.None;
        }
    }
}
