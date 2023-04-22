using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Housing;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.Map
{
    public interface IResidenceMapInstance : IMapInstance
    {
        // <summary>
        /// Initialise <see cref="IResidenceMapInstance"/> with <see cref="IResidence"/>.
        /// </summary>
        void Initialise(IResidence residence);

        /// <summary>
        /// Add child <see cref="IResidence"/> to parent <see cref="IResidence"/>.
        /// </summary>
        void AddChild(IResidence residence, bool temporary);

        /// <summary>
        /// Remove child <see cref="IResidence"/> to parent <see cref="IResidence"/>.
        /// </summary>
        void RemoveChild(IResidence residence);

        /// <summary>
        /// Crate all placed <see cref="IDecor"/>.
        /// </summary>
        void CrateAllDecor(TargetResidence targetResidence, IPlayer player);

        /// <summary>
        /// Handle <see cref="IDecor"/> update (create, move or delete).
        /// </summary>
        void DecorUpdate(IPlayer player, ClientHousingDecorUpdate housingDecorUpdate);

        /// <summary>
        /// Create and add <see cref="IDecor"/> from supplied <see cref="HousingDecorInfoEntry"/> to your crate.
        /// </summary>
        void DecorCreate(IResidence residence, HousingDecorInfoEntry entry, uint quantity);

        /// <summary>
        /// Remove an existing <see cref="IDecor"/> from <see cref="IResidence"/>.
        /// </summary>
        void DecorDelete(IResidence residence, IDecor decor);

        /// <summary>
        /// Create a new <see cref="IDecor"/> from an existing <see cref="IDecor"/> for <see cref="IResidence"/>.
        /// </summary>
        /// <remarks>
        /// Copies all data from the source <see cref="IDecor"/> with a new id.
        /// </remarks>
        void DecorCopy(IResidence residence, IDecor decor);

        /// <summary>
        /// Rename <see cref="IResidence"/> with supplied name.
        /// </summary>
        void RenameResidence(IPlayer player, TargetResidence targetResidence, string name);

        /// <summary>
        /// Rename <see cref="IResidence"/> with supplied name.
        /// </summary>
        void RenameResidence(IResidence residence, string name);

        /// <summary>
        /// Remodel <see cref="IResidence"/>.
        /// </summary>
        void Remodel(TargetResidence targetResidence, IPlayer player, ClientHousingRemodel housingRemodel);

        /// <summary>
        /// UpdateResidenceFlags <see cref="IResidence"/>.
        /// </summary>
        void UpdateResidenceFlags(TargetResidence targetResidence, IPlayer player, ClientHousingFlagsUpdate flagsUpdate);
    }
}