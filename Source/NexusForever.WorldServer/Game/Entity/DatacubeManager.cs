using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Shared;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NetworkDatacube = NexusForever.WorldServer.Network.Message.Model.Shared.Datacube;

namespace NexusForever.WorldServer.Game.Entity
{
    public class DatacubeManager : ISaveCharacter, IUpdate
    {
        private readonly Player player;
        public List<Datacube> DatacubeData { get; } = new List<Datacube>();
        public List<Datacube> DatacubeVolumeData { get; } = new List<Datacube>();

        private DatacubeDataSaveMask SaveMask { get; set; }

        public DatacubeManager(Player owner, Character characterModel)
        {
            player = owner;
            
            foreach(CharacterDatacube datacube in characterModel.CharacterDatacube)
            {
                if ((DatacubeType)datacube.Type == DatacubeType.Journal)
                    DatacubeVolumeData.Add(new Datacube(datacube));
                else
                    DatacubeData.Add(new Datacube(datacube));
            }
        }

        public void AddDatacube(Datacube datacube)
        {
            DatacubeData.Add(datacube);
            SendDatacube(datacube);
        }

        public void AddDatacubeVolume(Datacube datacube)
        {
            DatacubeVolumeData.Add(datacube);
            SendDatacubeVolume(datacube);
        }

        public void Save(CharacterContext context)
        {
            if (SaveMask == DatacubeDataSaveMask.None) 
                return;

            if ((SaveMask & DatacubeDataSaveMask.Datacube) != 0)
            {
                foreach(Datacube datacube in DatacubeData)
                {
                    if (datacube.saveMask == DatacubeSaveMask.None)
                        continue;

                    var model = new CharacterDatacube
                    {
                        Id = player.CharacterId,
                        Type = (byte)DatacubeType.Datacube,
                        DatacubeId = datacube.DatacubeId
                    };

                    if ((datacube.saveMask & DatacubeSaveMask.Create) != 0)
                    {
                        model.Progress = datacube.Progress;
                        context.Add(model);
                    }
                    else if ((datacube.saveMask & DatacubeSaveMask.Modify) != 0)
                    {
                        EntityEntry<CharacterDatacube> entity = context.Attach(model);
                        model.Progress = datacube.Progress;
                        entity.Property(p => p.Progress).IsModified = true;
                    }

                    datacube.saveMask = DatacubeSaveMask.None;
                }
            }

            if ((SaveMask & DatacubeDataSaveMask.DatacubeVolume) != 0)
            {
                foreach(Datacube datacube in DatacubeVolumeData)
                {
                    if (datacube.saveMask == DatacubeSaveMask.None)
                        continue;

                    var model = new CharacterDatacube
                    {
                        Id = player.CharacterId,
                        Type = (byte)DatacubeType.Journal,
                        DatacubeId = datacube.DatacubeId
                    };

                    if ((datacube.saveMask & DatacubeSaveMask.Create) != 0)
                    {
                        model.Progress = datacube.Progress;
                        context.Add(model);
                    }
                    else if ((datacube.saveMask & DatacubeSaveMask.Modify) != 0)
                    {
                        EntityEntry<CharacterDatacube> entity = context.Attach(model);
                        model.Progress = datacube.Progress;
                        entity.Property(p => p.Progress).IsModified = true;
                    }

                    datacube.saveMask = DatacubeSaveMask.None;
                }
            }

            SaveMask = DatacubeDataSaveMask.None;
        }

        public void Update(double lastTick)
        {
        }

        private void SendDatacubeList()
        {
            List<NetworkDatacube> datacubeData = new List<NetworkDatacube>();
            List<NetworkDatacube> datacubeVolumeData = new List<NetworkDatacube>();

            foreach (Datacube datacube in DatacubeData)
                datacubeData.Add(BuildNetworkDatacube(datacube));

            foreach (Datacube datacube in DatacubeVolumeData)
                datacubeVolumeData.Add(BuildNetworkDatacube(datacube));

            player.Session.EnqueueMessageEncrypted(new ServerDatacubeUpdateList
            {
                DatacubeData        = datacubeData,
                DatacubeVolumeData  = datacubeVolumeData
            });
        }

        public void SendDatacube(Datacube datacube)
        {
            NetworkDatacube networkDatacube = BuildNetworkDatacube(datacube);
            SaveMask |= DatacubeDataSaveMask.Datacube;

            player.Session.EnqueueMessageEncrypted(new ServerDatacubeUpdate
            {
                DatacubeData = networkDatacube
            });
        }

        public void SendDatacubeVolume(Datacube datacube)
        {
            NetworkDatacube networkDatacube = BuildNetworkDatacube(datacube);
            SaveMask |= DatacubeDataSaveMask.DatacubeVolume;

            player.Session.EnqueueMessageEncrypted(new ServerDatacubeVolumeUpdate
            {
                DatacubeVolumeData = networkDatacube
            });
        }

        public void SendInitialPackets()
        {
            SendDatacubeList();
        }

        private NetworkDatacube BuildNetworkDatacube(Datacube datacube)
        {
            var networkDatacube = new NetworkDatacube
            {
                DatacubeId  = datacube.DatacubeId, 
                Progress    = datacube.Progress 
            };

            return networkDatacube;
        }
    }
}
