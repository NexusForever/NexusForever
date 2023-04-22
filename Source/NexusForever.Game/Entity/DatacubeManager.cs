using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Entity
{
    public class DatacubeManager : IDatacubeManager
    {
        private static uint DatacubeHash(ushort id, DatacubeType type)
        {
            return (uint)id << 16 | (uint)type;
        }

        private readonly IPlayer player;
        private readonly Dictionary<uint, IDatacube> datacubes = new();

        /// <summary>
        /// Create a new <see cref="IDatacubeManager"/> from an existing database model.
        /// </summary>
        public DatacubeManager(IPlayer owner, CharacterModel characterModel)
        {
            player = owner;

            foreach (CharacterDatacubeModel model in characterModel.Datacube)
            {
                var datacube = new Datacube(player, model);
                uint hash = DatacubeHash(datacube.Id, datacube.Type);
                datacubes.Add(hash, datacube);
            }
        }

        public void Save(CharacterContext context)
        {
            foreach (IDatacube datacube in datacubes.Values)
                datacube.Save(context);
        }

        /// <summary>
        /// Return <see cref="IDatacube"/> with supplied id and <see cref="DatacubeType"/>.
        /// </summary>
        public IDatacube GetDatacube(ushort id, DatacubeType type)
        {
            uint hash = DatacubeHash(id, type);
            return datacubes.TryGetValue(hash, out IDatacube datacube) ? datacube : null;
        }

        /// <summary>
        /// Create a new <see cref="IDatacube"/> of type <see cref="DatacubeType.Datacube"/> with supplied id and progress.
        /// </summary>
        public void AddDatacube(ushort id, uint progress)
        {
            if (GameTableManager.Instance.Datacube.GetEntry(id) == null)
                throw new ArgumentException();

            var datacube = new Datacube(player, id, DatacubeType.Datacube, progress);
            datacubes.Add(DatacubeHash(id, DatacubeType.Datacube), datacube);

            SendDatacube(datacube);
        }

        /// <summary>
        /// Create a new <see cref="IDatacube"/> of type <see cref="DatacubeType.Journal"/> with supplied id and progress.
        /// </summary>
        public void AddDatacubeVolume(ushort id, uint progress)
        {
            if (GameTableManager.Instance.DatacubeVolume.GetEntry(id) == null)
                throw new ArgumentException();

            var datacube = new Datacube(player, id, DatacubeType.Journal, progress);
            datacubes.Add(DatacubeHash(id, DatacubeType.Journal), datacube);

            SendDatacubeVolume(datacube);
        }

        public void SendInitialPackets()
        {
            var datacubeUpdateList = new ServerDatacubeUpdateList();
            foreach (IDatacube datacube in datacubes.Values)
            {
                if (datacube.Type == DatacubeType.Datacube)
                    datacubeUpdateList.DatacubeData.Add(datacube.Build());
                else
                    datacubeUpdateList.DatacubeVolumeData.Add(datacube.Build());
            }

            player.Session.EnqueueMessageEncrypted(datacubeUpdateList);
        }

        public void SendDatacube(IDatacube datacube)
        {
            player.Session.EnqueueMessageEncrypted(new ServerDatacubeUpdate
            {
                DatacubeData = datacube.Build()
            });
        }

        public void SendDatacubeVolume(IDatacube datacube)
        {
            player.Session.EnqueueMessageEncrypted(new ServerDatacubeVolumeUpdate
            {
                DatacubeVolumeData = datacube.Build()
            });
        }
    }
}
