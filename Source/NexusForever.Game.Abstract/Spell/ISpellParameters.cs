using NexusForever.Game.Abstract.CSI;
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
        Position TargetPosition { get; set; }
        uint PositionalUnitId { get; set; }
        ushort TaxiNode { get; set; }
        uint ThresholdValue { get; set; }
        bool IsProxy { get; set; }
        bool ForceCancelOnly { get; set; }
        IClientSideInteraction ClientSideInteraction { get; set; }
        Action<ISpellParameters> CompleteAction { get; set; }
        int CastTimeOverride { get; set; }
        double CooldownOverride { get; set; }
    }
}