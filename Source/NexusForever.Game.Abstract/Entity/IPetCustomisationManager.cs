using NexusForever.Database.Character;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IPetCustomisationManager : IDatabaseCharacter
    {
        /// <summary>
        /// Unlock pet flair with supplied id.
        /// </summary>
        void UnlockFlair(ushort id);

        /// <summary>
        /// Add or update equipped pet flair at supplied index for <see cref="PetType"/> and object id.
        /// </summary>
        void AddCustomisation(PetType type, uint objectId, ushort index, ushort flairId);

        /// <summary>
        /// Return <see cref="IPetCustomisation"/> for supplied <see cref="PetType"/> and object id.
        /// </summary>
        IPetCustomisation GetCustomisation(PetType type, uint objectId);

        void SendInitialPackets();
    }
}