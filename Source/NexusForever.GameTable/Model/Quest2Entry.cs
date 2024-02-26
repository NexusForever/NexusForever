namespace NexusForever.GameTable.Model
{
    public class Quest2Entry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdTitle { get; set; }
        public uint LocalizedTextIdText { get; set; }
        public uint Flags { get; set; }
        public uint ConLevel { get; set; }
        public uint Type { get; set; }
        public uint PrerequisiteLevel { get; set; }
        public uint PrerequisiteFlags { get; set; }
        [GameTableFieldArray(3u)]
        public uint[] PrerequisiteQuests { get; set; }
        public uint PrerequisiteRace { get; set; }
        public uint PrerequisiteItem { get; set; }
        public uint QuestPlayerFactionEnum { get; set; }
        public uint WorldZoneId { get; set; }
        public uint LocalizedTextIdCompletionOverride { get; set; }
        public uint RewardXpOverride { get; set; }
        public uint RewardCashOverride { get; set; }
        [GameTableFieldArray(6u)]
        public uint[] PushedItemIds { get; set; }
        [GameTableFieldArray(6u)]
        public uint[] PushedItemCounts { get; set; }
        [GameTableFieldArray(6u)]
        public uint[] Objectives { get; set; }
        public uint LocalizedTextIdGiverTextUnknown { get; set; }
        public uint LocalizedTextIdGiverTextAccepted { get; set; }
        public uint LocalizedTextIdReceiverTextAccepted { get; set; }
        public uint LocalizedTextIdReceiverTextAchieved { get; set; }
        public uint LocalizedTextIdGiverSayAccepted { get; set; }
        public uint LocalizedTextIdReceiverSayCompleted { get; set; }
        public uint PrerequisiteClass { get; set; }
        public uint GroupId { get; set; }
        public uint FactionIdPreq0 { get; set; }
        public uint FactionIdPreq01 { get; set; }
        public uint FactionIdPreq02 { get; set; }
        public uint FactionLevelPreq0 { get; set; }
        public uint FactionLevelPreq01 { get; set; }
        public uint FactionLevelPreq02 { get; set; }
        public bool FactionLevelCompPreq0 { get; set; }
        public bool FactionLevelCompPreq01 { get; set; }
        public bool FactionLevelCompPreq02 { get; set; }
        public uint QuestIdExclusionPreq0 { get; set; }
        public uint QuestIdExclusionPreq1 { get; set; }
        public uint QuestIdExclusionPreq2 { get; set; }
        public uint LocalizedTextIdAcceptResponse { get; set; }
        public uint LocalizedTextIdCompleteResponse { get; set; }
        public uint WorldLocation2IdReceiver { get; set; }
        public uint WorldLocation2IdAltReceiver00 { get; set; }
        public uint WorldLocation2IdAltReceiver01 { get; set; }
        public uint WorldLocation2IdAltReceiver02 { get; set; }
        public uint PrerequisiteIdAltReceiver00 { get; set; }
        public uint PrerequisiteIdAltReceiver01 { get; set; }
        public uint PrerequisiteIdAltReceiver02 { get; set; }
        public uint QuestDirectionIdAltReceiver00 { get; set; }
        public uint QuestDirectionIdAltReceiver01 { get; set; }
        public uint QuestDirectionIdAltReceiver02 { get; set; }
        public uint LocalizedTextIdCompletedSummary { get; set; }
        public uint LocalizedTextIdGiverIncompleteResponse { get; set; }
        public uint LocalizedTextIdReceiverIncompleteResponse { get; set; }
        public uint Quest2DifficultyId { get; set; }
        public uint MaxTimeAllowedMS { get; set; }
        public uint PrerequisiteId { get; set; }
        public uint QuestShareEnum { get; set; }
        public uint SubMissionPathType { get; set; }
        public uint LocalizedTextIdCompletedObjectiveShort { get; set; }
        public uint Quest2SubTypeId { get; set; }
        public uint LocalizedTextIdMoreInfoSay00 { get; set; }
        public uint LocalizedTextIdMoreInfoSay01 { get; set; }
        public uint LocalizedTextIdMoreInfoSay02 { get; set; }
        public uint LocalizedTextIdMoreInfoSay03 { get; set; }
        public uint LocalizedTextIdMoreInfoSay04 { get; set; }
        public uint LocalizedTextIdMoreInfoText00 { get; set; }
        public uint LocalizedTextIdMoreInfoText01 { get; set; }
        public uint LocalizedTextIdMoreInfoText02 { get; set; }
        public uint LocalizedTextIdMoreInfoText03 { get; set; }
        public uint LocalizedTextIdMoreInfoText04 { get; set; }
        public uint VirtualItemIdPushed00 { get; set; }
        public uint VirtualItemIdPushed01 { get; set; }
        public uint VirtualItemIdPushed02 { get; set; }
        public uint VirtualItemIdPushed03 { get; set; }
        public uint VirtualItemPushedCount00 { get; set; }
        public uint VirtualItemPushedCount01 { get; set; }
        public uint VirtualItemPushedCount02 { get; set; }
        public uint VirtualItemPushedCount03 { get; set; }
        public uint VirtualItemPushedObjectiveFlagsEnum00 { get; set; }
        public uint VirtualItemPushedObjectiveFlagsEnum01 { get; set; }
        public uint VirtualItemPushedObjectiveFlagsEnum02 { get; set; }
        public uint VirtualItemPushedObjectiveFlagsEnum03 { get; set; }
        public uint QuestDirectionIdCompletion { get; set; }
        public uint Faction2IdRewardReputation00 { get; set; }
        public uint Faction2IdRewardReputation01 { get; set; }
        public float RewardReputationOverride00 { get; set; }
        public float RewardReputationOverride01 { get; set; }
        public uint QuestCategoryId { get; set; }
        public uint LocalizedTextIdGiverSayDecline { get; set; }
        public uint PeriodicQuestGroupId { get; set; }
        public uint PeriodicQuestWeight { get; set; }
        public uint QuestRepeatPeriodEnum { get; set; }
        public uint QuestContentFinderTypeEnum { get; set; }
        public uint GroupSize { get; set; }
    }
}
