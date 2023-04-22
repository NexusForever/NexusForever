﻿using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Prerequisite;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Quest
{
    public class CommunicatorMessage : ICommunicatorMessage
    {
        public uint Id => entry.Id;
        public ushort QuestId => (ushort)entry.QuestIdDelivered;

        private readonly CommunicatorMessagesEntry entry;

        /// <summary>
        /// Create a new <see cref="ICommunicatorMessage"/> with supplied <see cref="CommunicatorMessagesEntry"/>.
        /// </summary>
        public CommunicatorMessage(CommunicatorMessagesEntry entry)
        {
            this.entry = entry;
        }

        /// <summary>
        /// Checks if <see cref="IPlayer"/> meets the required conditions for this quest to be added to their communicator.
        /// </summary>
        public bool Meets(IPlayer player)
        {
            if (entry.WorldId != 0u && entry.WorldId != player.Map.Entry.Id)
                return false;

            // TODO: Skip this check until we have better WorldZoneId tracking
            // It also appears as though this is more of a "Trigger when Player gets here".
            // It's plausible this check should be "Has this player been to this zone id?".
            //if (entry.WorldZoneId != 0u && entry.WorldZoneId != player.Zone.Id)
            //    return false;

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

        /// <summary>
        /// Send communicator message to <see cref="IGameSession"/>.
        /// </summary>
        public void Send(IGameSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCommunicatorMessage
            {
                CommunicatorId = (ushort)entry.Id
            });
        }
    }
}
