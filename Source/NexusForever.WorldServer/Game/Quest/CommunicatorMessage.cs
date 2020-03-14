using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Prerequisite;
using NexusForever.WorldServer.Game.Quest.Static;

namespace NexusForever.WorldServer.Game.Quest
{
    public class CommunicatorMessage
    {
        public ushort Id => (ushort)entry.QuestIdDelivered;

        private readonly CommunicatorMessagesEntry entry;

        /// <summary>
        /// Create a new <see cref="CommunicatorMessage"/> with supplied <see cref="CommunicatorMessagesEntry"/>.
        /// </summary>
        public CommunicatorMessage(CommunicatorMessagesEntry entry)
        {
            this.entry = entry;
        }

        /// <summary>
        /// Checks if <see cref="Player"/> meets the required conditions for this quest to be added to their communicator.
        /// </summary>
        public bool Meets(Player player)
        {
            if (entry.WorldId != 0u && entry.WorldId != player.Map.Entry.Id)
                return false;

            if (entry.WorldZoneId != 0u && entry.WorldZoneId != player.Zone.Id)
                return false;

            if (entry.MinLevel != 0u && player.Level < entry.MaxLevel)
                return false;

            if (entry.MaxLevel != 0u && player.Level > entry.MaxLevel)
                return false;

            for (int i = 0; i < entry.Quests.Length; i++)
            {
                ushort questId = (ushort)entry.Quests[i];
                if (questId == 0)
                    continue;

                if (player.QuestManager.GetQuestState(questId) != (QuestState)entry.States[i])
                    return false;
            }

            if (entry.FactionId != 0u && (Faction)entry.FactionId != player.Faction1)
                return false;

            if (entry.ClassId != 0u && (Class)entry.ClassId != player.Class)
                return false;

            // TODO: reputation

            if (entry.PrerequisiteId != 0u && !PrerequisiteManager.Instance.Meets(player, entry.PrerequisiteId))
                return false;

            return true;
        }
    }
}
