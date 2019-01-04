using NexusForever.WorldServer.Game.Entity.Static;
using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterPath
    {
        public ulong Id { get; set; }
        public string PathName { get; set; }
        public bool Unlocked { get; set; }
        public uint TotalXp { get; set; }
        public byte LevelRewarded { get; set; }

        public Character IdNavigation { get; set; }
    }
}
