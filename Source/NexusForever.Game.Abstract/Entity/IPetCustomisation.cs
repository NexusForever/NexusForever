using NexusForever.Database.Character;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IPetCustomisation : IDatabaseCharacter, INetworkBuildable<PetCustomisation>, IEnumerable<PetFlairEntry>
    {
        ulong Owner { get; }
        PetType Type { get; }
        uint ObjectId { get; }
        string Name { get; set; }

        /// <summary>
        /// Add or update flair from supplied <see cref="PetFlairEntry"/> at index.
        /// </summary>
        void AddFlair(ushort index, PetFlairEntry entry);
    }
}