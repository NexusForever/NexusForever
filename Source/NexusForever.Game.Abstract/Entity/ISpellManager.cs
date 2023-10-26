using NexusForever.Database.Character;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Network.World.Message.Static;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ISpellManager : IDatabaseCharacter, IUpdate
    {
        /// <summary>
        /// Index of the active <see cref="IActionSet"/>.
        /// </summary>
        byte ActiveActionSet { get; }

        void GrantSpells();

        /// <summary>
        /// Returns <see cref="ICharacterSpell"/> for an existing spell.
        /// </summary>
        ICharacterSpell GetSpell(uint spell4BaseId);

        /// <summary>
        /// Add a new <see cref="ICharacterSpell"/> created from supplied spell base id and tier.
        /// </summary>
        void AddSpell(uint spell4BaseId, byte tier = 1);

        /// <summary>
        /// Removes an existing spell (used for AMPs)
        /// </summary>
        void RemoveSpell(uint spell4BaseId);

        /// <summary>
        /// Update existing <see cref="ICharacterSpell"/> with supplied tier. The base tier will be updated if no action set index is supplied.
        /// </summary>
        void UpdateSpell(uint spell4BaseId, byte tier, byte? actionSetIndex);

        /// <summary>
        /// Return the tier for supplied spell.
        /// This will either be the <see cref="IActionSetShortcut"/> tier if placed in the active <see cref="IActionSet"/> or base tier if not.
        /// </summary>
        byte GetSpellTier(uint spell4BaseId);

        List<ICharacterSpell> GetPets();

        /// <summary>
        /// Return spell cooldown for supplied spell id in seconds.
        /// </summary>
        double GetSpellCooldown(uint spellId);

        /// <summary>
        /// Set spell cooldown in seconds for supplied spell id.
        /// </summary>
        void SetSpellCooldown(uint spell4Id, double cooldown);

        void ResetAllSpellCooldowns();
        double GetGlobalSpellCooldown();
        void SetGlobalSpellCooldown(double cooldown);

        /// <summary>
        /// Return <see cref="IActionSet"/> at supplied index.
        /// </summary>
        IActionSet GetActionSet(byte actionSetIndex);

        /// <summary>
        /// Update active <see cref="IActionSet"/> with supplied index, returned <see cref="SpecError"/> is sent to the client.
        /// </summary>
        SpecError SetActiveActionSet(byte value);

        void ApplyAmps();

        void SendInitialPackets();
        void SendServerAbilityPoints();
    }
}