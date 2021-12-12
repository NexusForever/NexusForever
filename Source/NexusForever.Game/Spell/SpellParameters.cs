using NexusForever.Game.Abstract.CSI;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Network.World.Entity;

namespace NexusForever.Game.Spell
{
    public class SpellParameters : ISpellParameters
    {
        public ICharacterSpell CharacterSpell { get; set; }
        public ISpellInfo SpellInfo { get; set; }
        public ISpellInfo ParentSpellInfo { get; set; }
        public ISpellInfo RootSpellInfo { get; set; }
        public bool UserInitiatedSpellCast { get; set; } = true;
        public uint PrimaryTargetId { get; set; }
        public Position TargetPosition { get; set; }
        public uint PositionalUnitId { get; set; }
        public ushort TaxiNode { get; set; }
        public uint ThresholdValue { get; set; }
        public bool IsProxy { get; set; }
        public bool ForceCancelOnly { get; set; }
        public IClientSideInteraction ClientSideInteraction { get; set; }
        public Action<ISpellParameters> CompleteAction { get; set; }
        public int CastTimeOverride { get; set; } = -1;
        public double CooldownOverride { get; set; } = 0d;
    }
}
