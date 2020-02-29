using System;
using System.Collections.Generic;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NetworkDatacube = NexusForever.WorldServer.Network.Message.Model.Shared.Datacube;

namespace NexusForever.WorldServer.Game.Entity
{
    public class DatacubeManager : ISaveCharacter
    {
        private static uint DatacubeHash(ushort id, DatacubeType type)
        {
            return ((uint)id << 16) | (uint)type;
        }

        private readonly Player player;
        private readonly Dictionary<uint, Datacube> datacubes = new Dictionary<uint, Datacube>();

        /// <summary>
        /// Create a new <see cref="DatacubeManager"/> from an existing database model.
        /// </summary>
        public DatacubeManager(Player owner, CharacterModel characterModel)
        {
            player = owner;
            
            foreach (CharacterDatacubeModel model in characterModel.Datacube)
            {
                var datacube = new Datacube(model);
                uint hash = DatacubeHash(datacube.Id, datacube.Type);
                datacubes.Add(hash, datacube);
            }
        }

        public void Save(CharacterContext context)
        {
            foreach (Datacube datacube in datacubes.Values)
                datacube.Save(context, player.CharacterId);
        }

        /// <summary>
        /// Return <see cref="Datacube"/> with supplied id and <see cref="DatacubeType"/>.
        /// </summary>
        public Datacube GetDatacube(ushort id, DatacubeType type)
        {
            uint hash = DatacubeHash(id, type);
            return datacubes.TryGetValue(hash, out Datacube datacube) ? datacube : null;
        }

        /// <summary>
        /// Create a new <see cref="Datacube"/> of type <see cref="DatacubeType.Datacube"/> with supplied id and progress.
        /// </summary>
        public void AddDatacube(ushort id, uint progress)
        {
            if (GameTableManager.Instance.Datacube.GetEntry(id) == null)
                throw new ArgumentException();
            
            var datacube = new Datacube(id, DatacubeType.Datacube, progress);
            datacubes.Add(DatacubeHash(id, DatacubeType.Datacube), datacube);

            SendDatacube(datacube);
        }

        /// <summary>
        /// Create a new <see cref="Datacube"/> of type <see cref="DatacubeType.Journal"/> with supplied id and progress.
        /// </summary>
        public void AddDatacubeVolume(ushort id, uint progress)
        {
            if (GameTableManager.Instance.DatacubeVolume.GetEntry(id) == null)
                throw new ArgumentException();

            var datacube = new Datacube(id, DatacubeType.Journal, progress);
            datacubes.Add(DatacubeHash(id, DatacubeType.Journal), datacube);

            SendDatacubeVolume(datacube);
        }

        public void SendInitialPackets()
        {
            var datacubeUpdateList = new ServerDatacubeUpdateList();
            foreach (Datacube datacube in datacubes.Values)
            {
                if (datacube.Type == DatacubeType.Datacube)
                    datacubeUpdateList.DatacubeData.Add(BuildNetworkDatacube(datacube));
                else
                    datacubeUpdateList.DatacubeVolumeData.Add(BuildNetworkDatacube(datacube));
            }

            player.Session.EnqueueMessageEncrypted(datacubeUpdateList);
        }

        public void SendDatacube(Datacube datacube)
        {
            player.Session.EnqueueMessageEncrypted(new ServerDatacubeUpdate
            {
                DatacubeData = BuildNetworkDatacube(datacube)
            });
        }

        public void SendDatacubeVolume(Datacube datacube)
        {
            player.Session.EnqueueMessageEncrypted(new ServerDatacubeVolumeUpdate
            {
                DatacubeVolumeData = BuildNetworkDatacube(datacube)
            });
        }

        private NetworkDatacube BuildNetworkDatacube(Datacube datacube)
        {
            return new NetworkDatacube
            {
                DatacubeId = datacube.Id,
                Progress   = datacube.Progress
            };
        }
    }
}
