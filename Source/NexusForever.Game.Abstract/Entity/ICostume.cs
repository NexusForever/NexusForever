using NexusForever.Database.Character;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ICostume : IDatabaseCharacter, INetworkBuildable<Costume>, IEnumerable<ICostumeItem>
    {
        ulong Owner { get; }
        byte Index { get; }
        uint Mask { get; set; }

        /// <summary>
        /// Return <see cref="ICostumeItem"/> at supplied index.
        /// </summary>
        ICostumeItem GetItem(CostumeItemSlot slot);

        /// <summary>
        /// Update <see cref="ICostume"/> from <see cref="ClientCostumeSave"/>.
        /// </summary>
        void Update(ClientCostumeSave costumeSave);
    }
}