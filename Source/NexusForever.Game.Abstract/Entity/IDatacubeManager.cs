using NexusForever.Database.Character;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IDatacubeManager : IDatabaseCharacter
    {
        /// <summary>
        /// Return <see cref="IDatacube"/> with supplied id and <see cref="DatacubeType"/>.
        /// </summary>
        IDatacube GetDatacube(ushort id, DatacubeType type);

        /// <summary>
        /// Create a new <see cref="IDatacube"/> of type <see cref="DatacubeType.Datacube"/> with supplied id and progress.
        /// </summary>
        void AddDatacube(ushort id, uint progress);

        /// <summary>
        /// Create a new <see cref="IDatacube"/> of type <see cref="DatacubeType.Journal"/> with supplied id and progress.
        /// </summary>
        void AddDatacubeVolume(ushort id, uint progress);

        void SendInitialPackets();
        void SendDatacube(IDatacube datacube);
        void SendDatacubeVolume(IDatacube volume);
    }
}