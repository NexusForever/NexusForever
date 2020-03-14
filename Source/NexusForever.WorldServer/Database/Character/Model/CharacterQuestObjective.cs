using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterQuestObjective
    {
        public ulong Id { get; set; }
        public ushort QuestId { get; set; }
        public byte Index { get; set; }
        public uint Progress { get; set; }
        public uint? Timer { get; set; }

        public virtual CharacterQuest CharacterQuest { get; set; }
    }
}
