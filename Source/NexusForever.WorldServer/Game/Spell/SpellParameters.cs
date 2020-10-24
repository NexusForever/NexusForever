using NexusForever.WorldServer.Game.CSI;
using NexusForever.WorldServer.Game.Entity;
using System;

namespace NexusForever.WorldServer.Game.Spell
{
    public class SpellParameters
    {
        public CharacterSpell CharacterSpell { get; set; }
        public SpellInfo SpellInfo { get; set; }
        public SpellInfo ParentSpellInfo { get; set; }
        public SpellInfo RootSpellInfo { get; set; }
        public bool UserInitiatedSpellCast { get; set; }
        public uint PrimaryTargetId { get; set; }
        public Position Position { get; set; }
        public ushort TaxiNode { get; set; }
        public ClientSideInteraction ClientSideInteraction { get; set; }
        public Action<SpellParameters> CompleteAction { get; set; }
        public int CastTimeOverride { get; set; } = -1;
    }
}
