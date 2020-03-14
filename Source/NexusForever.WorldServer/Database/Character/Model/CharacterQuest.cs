using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterQuest
    {
        public CharacterQuest()
        {
            CharacterQuestObjective = new HashSet<CharacterQuestObjective>();
        }

        public ulong Id { get; set; }
        public ushort QuestId { get; set; }
        public byte State { get; set; }
        public byte Flags { get; set; }
        public uint? Timer { get; set; }
        public DateTime? Reset { get; set; }

        public virtual Character IdNavigation { get; set; }
        public virtual ICollection<CharacterQuestObjective> CharacterQuestObjective { get; set; }
    }
}
