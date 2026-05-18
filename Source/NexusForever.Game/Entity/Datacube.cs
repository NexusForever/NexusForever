using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NetworkDatacube = NexusForever.Network.World.Message.Model.Shared.Datacube;

namespace NexusForever.Game.Entity
{
    public class Datacube : IDatacube
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

        private readonly IPlayer player;

        /// <summary>
        /// Create a new <see cref="IDatacube"/> from the supplied id, <see cref="DatacubeType"/> and progress.
        /// </summary>
        public Datacube(IPlayer player, ushort id, DatacubeType type, uint progress)
        {
            this.player = player;

            Id       = id;
            Type     = type;
            Progress = progress;

            saveMask = DatacubeSaveMask.Create;
        }

        /// <summary>
        /// Create a new <see cref="IDatacube"/> from an existing database model.
        /// </summary>
        public Datacube(IPlayer player, CharacterDatacubeModel model)
        {
            this.player = player;

            Id       = model.Datacube;
            Type     = (DatacubeType)model.Type;
            Progress = model.Progress;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == DatacubeSaveMask.None)
                return;

            var model = new CharacterDatacubeModel
            {
                Id       = player.CharacterId,
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

        public NetworkDatacube Build()
        {
            return new NetworkDatacube
            {
                DatacubeId = Id,
                Progress   = Progress
            };
        }
    }
}
