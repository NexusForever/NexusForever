using System;
using System.Collections.Generic;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Database.World.Model
{
    public partial class EntityStat
    {
        public uint Id { get; set; }
        public byte Stat { set ; get; }
        public float Value { get; set; }

        public Entity IdNavigation { get; set; }
    }
}
