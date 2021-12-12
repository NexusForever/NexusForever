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
        /// <summary>
        /// Index of the active Innate ability.
        /// </summary>
        byte InnateIndex { get; set; }

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
        /// Return remaining cooldown for supplied cooldown ID.
        /// </summary>
        double GetSpellCooldownByCooldownId(uint spell4CooldownId);

        /// <summary>
        /// Set spell cooldown in seconds for supplied spell id.
        /// </summary>
        /// 
        void SetSpellCooldown(ISpellInfo spellInfo, double cooldown, bool emit = false);

        /// <summary>
        /// Set spell cooldown in seconds for supplied spell id.
        /// </summary>
        void SetSpellCooldown(uint spell4Id, double cooldown, bool emit = false);

        /// <summary>
        /// Update all spell cooldowns that share a given cooldown ID
        /// </summary>
        void SetSpellCooldownByCooldownId(uint spell4CooldownId, double newCooldown);

        /// <summary>
        /// Update all spell cooldowns that share a given group ID
        /// </summary>
        void SetSpellCooldownByGroupId(uint groupId, double cooldown);

        /// <summary>
        /// Update all spell cooldowns that share a base spell ID
        /// </summary>
        void SetSpellCooldownByBaseSpell(uint spell4BaseId, uint type, double cooldown);

        void ResetAllSpellCooldowns();
        double GetGlobalSpellCooldown(uint globalEnum);
        void SetGlobalSpellCooldown(uint globalEnum, double cooldown);

        /// <summary>
        /// Return <see cref="IActionSet"/> at supplied index.
        /// </summary>
        IActionSet GetActionSet(byte actionSetIndex);

        /// <summary>
        /// Update active <see cref="IActionSet"/> with supplied index, returned <see cref="SpecError"/> is sent to the client.
        /// </summary>
        SpecError SetActiveActionSet(byte value);

        /// <summary>
        /// Set the supplied spell as the spell to Continuously Cast when GCD expires. Should only be called by a <see cref="ICharacterSpell"/>.
        /// </summary>
        void SetAsContinuousCast(ICharacterSpell spell);

        /// <summary>
        /// Update active Innate Ability with supplied index.
        /// </summary>
        void SetInnate(byte index);

        void SendInitialPackets();
        void SendServerAbilityPoints();
    }
}