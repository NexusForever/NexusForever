using System;
using System.Collections.Generic;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Database.World.Model
{
    public partial class EntityProperty
    {
        public uint Id { get; set; }
        public byte Property { set ; get; }
        public float BaseValue { get; set; }
        public float Value { get; set; }

        public Entity IdNavigation { get; set; }
    }
}
