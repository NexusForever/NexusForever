using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Quest.Static;

namespace NexusForever.WorldServer.Game.Quest
{
    public class QuestObjectiveInfo
    {
        public uint Id => Entry.Id;
        public QuestObjectiveType Type => (QuestObjectiveType)Entry.Type;

        public QuestObjectiveEntry Entry { get; }

        /// <summary>
        /// Create a new <see cref="QuestObjectiveInfo"/> with supplied <see cref="QuestObjectiveEntry"/>.
        /// </summary>
        public QuestObjectiveInfo(QuestObjectiveEntry entry)
        {
            Entry = entry;
        }

        /// <summary>
        /// Quest objective is sequential, previous quest objective must be complete.
        /// </summary>
        public bool IsSequential()
        {
            return ((QuestObjectiveFlags)Entry.Flags & QuestObjectiveFlags.Sequential) != 0;
        }

        /// <summary>
        /// Quest objective is hidden until previous objective is complete.
        /// </summary>
        public bool IsHidden()
        {
            return ((QuestObjectiveFlags)Entry.Flags & QuestObjectiveFlags.Hidden) != 0;
        }

        /// <summary>
        /// Quest objective is optional, doesn't need to be complete for quest to be achieved.
        /// </summary>
        public bool IsOptional()
        {
            return ((QuestObjectiveFlags)Entry.Flags & QuestObjectiveFlags.Optional) != 0;
        }

        public bool HasUnknown0200()
        {
            return ((QuestObjectiveFlags)Entry.Flags & QuestObjectiveFlags.Unknown0200) != 0;
        }
    }
}
