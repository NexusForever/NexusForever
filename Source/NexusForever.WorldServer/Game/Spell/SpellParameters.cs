﻿using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Spell
{
    public class SpellParameters
    {
        public SpellInfo SpellInfo { get; set; }
        public SpellInfo ParentSpellInfo { get; set; }
        public SpellInfo RootSpellInfo { get; set; }
        public bool UserInitiatedSpellCast { get; set; }
        public uint PrimaryTargetId { get; set; }
    }
}
