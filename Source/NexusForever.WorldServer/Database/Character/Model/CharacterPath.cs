using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterPath
    {
        public ulong Id { get; set; }
        public byte ActivePath { get; set; }
        public ushort PathsUnlocked { get; set; }
        public uint SoldierXp { get; set; }
        public uint SettlerXp { get; set; }
        public uint ScientistXp { get; set; }
        public uint ExplorerXp { get; set; }
        public uint SoldierLevelRewarded { get; set; }
        public uint SettlerLevelRewarded { get; set; }
        public uint ScientistLevelRewarded { get; set; }
        public uint ExplorerLevelRewarded { get; set; }
        public DateTime PathActivatedTimestamp { get; set; }

        public Character Character { get; set; }
    }
}
