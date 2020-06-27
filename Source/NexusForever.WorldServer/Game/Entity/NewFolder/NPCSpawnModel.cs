using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Entity
{
    public class NPCSpawnModel
    {
        public uint Creature { get; set; }
        public string Description { get; set; }
        public uint Type { get; set; }
        public uint displayInfo { get; set; }
        public uint OutfitInfo { get; set; }
        public uint faction1 { get; set; }
        public uint faction2 { get; set; }
        public ulong activePropId { get; set; }
    }
}
