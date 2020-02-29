using System;
using System.Collections.Generic;

namespace NexusForever.Database.Character.Model
{
    public class CharacterQuestModel
    {
        public ulong Id { get; set; }
        public ushort QuestId { get; set; }
        public byte State { get; set; }
        public byte Flags { get; set; }
        public uint? Timer { get; set; }
        public DateTime? Reset { get; set; }

        public CharacterModel Character { get; set; }
        public ICollection<CharacterQuestObjectiveModel> QuestObjective { get; set; } = new HashSet<CharacterQuestObjectiveModel>();
    }
}
