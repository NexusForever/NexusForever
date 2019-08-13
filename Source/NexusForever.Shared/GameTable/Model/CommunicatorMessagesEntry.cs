namespace NexusForever.Shared.GameTable.Model
{
    public class CommunicatorMessagesEntry
    {
        public uint Id;
        public uint LocalizedTextIdMessage;
        public uint Delay;
        public uint Flags;
        public uint CreatureId;
        public uint WorldId;
        public uint WorldZoneId;
        public uint MinLevel;
        public uint MaxLevel;
        [GameTableFieldArray(3u)]
        public uint[] Quests;
        [GameTableFieldArray(3u)]
        public uint[] States;
        public uint FactionId;
        public uint ClassId;
        public uint RaceId;
        public uint FactionIdReputation;
        public uint ReputationMin;
        public uint ReputationMax;
        public uint QuestIdDelivered;
        public uint PrerequisiteId;
        public uint DisplayDuration;
        public uint CommunicatorMessagesIdNext;
        public uint CommunicatorPortraitPlacementEnum;
        public uint CommunicatorOverlayEnum;
        public uint CommunicatorBackgroundEnum;
    }
}
