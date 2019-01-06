using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterPath
    {
        public ulong Id { get; set; }
        public byte Path { get; set; }
        public byte Unlocked { get; set; }
        public uint TotalXp { get; set; }
        public byte LevelRewarded { get; set; }

        public Character IdNavigation { get; set; }
    }
}
