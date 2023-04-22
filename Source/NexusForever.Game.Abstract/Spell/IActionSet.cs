using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Static.Spell;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Game.Abstract.Spell
{
    public interface IActionSet : IDatabaseCharacter
    {
        ulong Owner { get; }
        byte Index { get; }
        byte TierPoints { get; }
        byte AmpPoints { get; }

        /// <summary>
        /// Collection of <see cref="IActionSetShortcut"/> contained in the <see cref="IActionSet"/>.
        /// </summary>
        IEnumerable<IActionSetShortcut> Actions { get; }

        /// <summary>
        /// Collection of <see cref="IActionSetAmp"/> contained in the <see cref="IActionSet"/>.
        /// </summary>
        IEnumerable<IActionSetAmp> Amps { get; }

        /// <summary>
        /// Return <see cref="IActionSetShortcut"/> at supplied <see cref="UILocation"/>.
        /// </summary>
        IActionSetShortcut GetShortcut(UILocation location);

        /// <summary>
        /// Return <see cref="IActionSetShortcut"/> with supplied <see cref="ShortcutType"/> and object id.
        /// </summary>
        IActionSetShortcut GetShortcut(ShortcutType type, uint objectId);

        /// <summary>
        /// Return <see cref="IActionSetAmp"/> with supplied id.
        /// </summary>
        IActionSetAmp GetAmp(ushort id);

        /// <summary>
        /// Add shortcut to <see cref="IActionSet"/> to supplied <see cref="UILocation"/>.
        /// </summary>
        void AddShortcut(UILocation location, ShortcutType type, uint objectId, byte tier);

        /// <summary>
        /// Add shortcut to <see cref="IActionSet"/> from an existing database model.
        /// </summary>
        void AddShortcut(CharacterActionSetShortcutModel model);

        /// <summary>
        /// Update a <see cref="ShortcutType.Spell"/> shortcut with supplied tier.
        /// </summary>
        void UpdateSpellShortcut(uint spell4BaseId, byte tier);

        /// <summary>
        /// Remove shortcut from <see cref="IActionSet"/> at supplied <see cref="UILocation"/>.
        /// </summary>
        void RemoveShortcut(UILocation location);

        /// <summary>
        /// Add AMP to <see cref="IActionSet"/> with supplied id.
        /// </summary>
        void AddAmp(ushort id);

        /// <summary>
        /// Add AMP to <see cref="IActionSet"/> from an existing database model.
        /// </summary>
        void AddAmp(CharacterActionSetAmpModel model);

        /// <summary>
        /// Remove one or more AMP's from <see cref="IActionSet"/> depending on supplied <see cref="AmpRespecType"/>.
        /// </summary>
        void RemoveAmp(AmpRespecType type, uint value);

        /// <summary>
        /// Build a network representation of the <see cref="IActionSetShortcut"/>'s in the <see cref="IActionSet"/>.
        /// </summary>
        ServerActionSet BuildServerActionSet();

        /// <summary>
        /// Build a network representation of the <see cref="IActionSetAmp"/>'s in the <see cref="IActionSet"/>.
        /// </summary>
        ServerAmpList BuildServerAmpList();
    }
}