using System;
using System.Collections.Generic;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterStat
    {
        public ulong Id { get; set; }
        public byte Stat { set ; get; }
        public float Value { get; set; }

        public Character IdNavigation { get; set; }
    }
}
