namespace NexusForever.GameTable.Model
{
    public class CommunicatorMessagesEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdMessage { get; set; }
        public uint Delay { get; set; }
        public uint Flags { get; set; }
        public uint CreatureId { get; set; }
        public uint WorldId { get; set; }
        public uint WorldZoneId { get; set; }
        public uint MinLevel { get; set; }
        public uint MaxLevel { get; set; }
        [GameTableFieldArray(3u)]
        public uint[] Quests { get; set; }
        [GameTableFieldArray(3u)]
        public uint[] States { get; set; }
        public uint FactionId { get; set; }
        public uint ClassId { get; set; }
        public uint RaceId { get; set; }
        public uint FactionIdReputation { get; set; }
        public uint ReputationMin { get; set; }
        public uint ReputationMax { get; set; }
        public uint QuestIdDelivered { get; set; }
        public uint PrerequisiteId { get; set; }
        public uint DisplayDuration { get; set; }
        public uint CommunicatorMessagesIdNext { get; set; }
        public uint CommunicatorPortraitPlacementEnum { get; set; }
        public uint CommunicatorOverlayEnum { get; set; }
        public uint CommunicatorBackgroundEnum { get; set; }
    }
}
