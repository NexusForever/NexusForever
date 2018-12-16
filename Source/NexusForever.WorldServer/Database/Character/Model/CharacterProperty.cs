using System;
using System.Collections.Generic;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterProperty
    {
        public ulong Id { get; set; }
        public byte Property { set ; get; }
        public float Base { get; set; }
        public float Value { get; set; }

        public Character IdNavigation { get; set; }
    }
}
