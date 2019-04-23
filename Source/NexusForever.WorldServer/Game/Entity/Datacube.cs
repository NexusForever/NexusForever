using System;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NetworkDatacube = NexusForever.WorldServer.Network.Message.Model.Shared.Datacube;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Datacube
    {
        public ushort DatacubeId { get; set; }
        public uint Progress { get; set; }

        public DatacubeSaveMask saveMask { get; set; }

        public Datacube()
        {
        }

        public Datacube(NetworkDatacube networkDatacube)
        {
            DatacubeId  = networkDatacube.DatacubeId;
            Progress    = networkDatacube.Progress;
        }

        public Datacube(CharacterDatacube characterDatacube)
        {
            DatacubeId  = characterDatacube.DatacubeId;
            Progress    = characterDatacube.Progress;
        }
    }
}
