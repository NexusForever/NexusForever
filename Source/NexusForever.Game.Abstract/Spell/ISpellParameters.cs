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
        /// Unique id the client assigned to this cast request, 0 if the cast did not come from a client
        /// message.
        /// </summary>
        /// <remarks>
        /// The client keys its own pending cast by this id and matches the server's confirm (0x07FD) and
        /// reject (0x07FC) replies against it.
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