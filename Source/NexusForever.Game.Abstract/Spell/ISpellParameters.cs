using NexusForever.Game.Abstract.CSI;
using NexusForever.Game.Abstract.Spell.Info;
using NexusForever.Network.World.Entity;

namespace NexusForever.Game.Abstract.Spell
{
    public interface ISpellParameters
    {
        ICharacterSpell CharacterSpell { get; set; }
        ISpellInfo SpellInfo { get; set; }
        ISpellInfo ParentSpellInfo { get; set; }
        ISpellInfo RootSpellInfo { get; set; }
        bool UserInitiatedSpellCast { get; set; }
        uint PrimaryTargetId { get; set; }

        /// <summary>
        /// Unique id the client assigned to this cast request, or 0 when the cast did not come from a
        /// client message.
        /// </summary>
        /// <remarks>
        /// The client creates its own pending cast before it sends the request and stamps it with this
        /// id. It then matches the server's confirm (0x07FD, ServerSpellStartClientInteraction) and its
        /// refusal (0x07FC, ServerSpellCastResult) against it. A reply carrying 0 matches nothing, so
        /// the client keeps the cast pending.
        /// Casts the server starts itself - proxies, continuous re-casts, threshold re-casts and NPC
        /// casts - leave this at 0 and are neither confirmed nor refused on the wire.
        /// </remarks>
        uint ClientSpellCastUniqueId { get; set; }
        Position TargetPosition { get; set; }
        uint PositionalUnitId { get; set; }
        ushort? TaxiNode { get; set; }
        ulong SpellCost { get; set; }
        uint ThresholdValue { get; set; }
        bool IsProxy { get; set; }
        bool ForceCancelOnly { get; set; }
        IClientSideInteraction ClientSideInteraction { get; set; }
        Action<ISpellParameters> CompleteAction { get; set; }
        int CastTimeOverride { get; set; }
        double CooldownOverride { get; set; }
    }
}