using NexusForever.Database.Character;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ICostumeManager : IDatabaseCharacter, IUpdate
    {
        sbyte CostumeIndex { get; }
        byte CostumeCap { get; }

        /// <summary>
        /// Return <see cref="ICostume"/> at supplied index.
        /// </summary>
        ICostume GetCostume(byte index);

        /// <summary>
        /// Validate then save or update <see cref="ICostume"/> from <see cref="ClientCostumeSave"/> packet.
        /// </summary>
        void SaveCostume(ClientCostumeSave costumeSave);

        /// <summary>
        /// Equip <see cref="ICostume"/> at supplied index.
        /// </summary>
        void SetCostume(int index);

        void SendInitialPackets();
    }
}