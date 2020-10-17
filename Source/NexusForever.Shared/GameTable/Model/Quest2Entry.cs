namespace NexusForever.Shared.GameTable.Model
{
    public class Quest2Entry
    {
        public uint Id;
        public uint LocalizedTextIdTitle;
        public uint LocalizedTextIdText;
        public uint Flags;
        public uint ConLevel;
        public uint Type;
        public uint PrerequisiteLevel;
        public uint PrerequisiteFlags;
        [GameTableFieldArray(3u)]
        public uint[] PrerequisiteQuests;
        public uint PrerequisiteRace;
        public uint PrerequisiteItem;
        public uint QuestPlayerFactionEnum;
        public uint WorldZoneId;
        public uint LocalizedTextIdCompletionOverride;
        public uint RewardXpOverride;
        public uint RewardCashOverride;
        [GameTableFieldArray(6u)]
        public uint[] PushedItemIds;
        [GameTableFieldArray(6u)]
        public uint[] PushedItemCounts;
        [GameTableFieldArray(6u)]
        public uint[] Objectives;
        public uint LocalizedTextIdGiverTextUnknown;
        public uint LocalizedTextIdGiverTextAccepted;
        public uint LocalizedTextIdReceiverTextAccepted;
        public uint LocalizedTextIdReceiverTextAchieved;
        public uint LocalizedTextIdGiverSayAccepted;
        public uint LocalizedTextIdReceiverSayCompleted;
        public uint PrerequisiteClass;
        public uint GroupId;
        public uint FactionIdPreq0;
        public uint FactionIdPreq01;
        public uint FactionIdPreq02;
        public uint FactionLevelPreq0;
        public uint FactionLevelPreq01;
        public uint FactionLevelPreq02;
        public bool FactionLevelCompPreq0;
        public bool FactionLevelCompPreq01;
        public bool FactionLevelCompPreq02;
        public uint QuestIdExclusionPreq0;
        public uint QuestIdExclusionPreq1;
        public uint QuestIdExclusionPreq2;
        public uint LocalizedTextIdAcceptResponse;
        public uint LocalizedTextIdCompleteResponse;
        public uint WorldLocation2IdReceiver;
        public uint WorldLocation2IdAltReceiver00;
        public uint WorldLocation2IdAltReceiver01;
        public uint WorldLocation2IdAltReceiver02;
        public uint PrerequisiteIdAltReceiver00;
        public uint PrerequisiteIdAltReceiver01;
        public uint PrerequisiteIdAltReceiver02;
        public uint QuestDirectionIdAltReceiver00;
        public uint QuestDirectionIdAltReceiver01;
        public uint QuestDirectionIdAltReceiver02;
        public uint LocalizedTextIdCompletedSummary;
        public uint LocalizedTextIdGiverIncompleteResponse;
        public uint LocalizedTextIdReceiverIncompleteResponse;
        public uint Quest2DifficultyId;
        public uint MaxTimeAllowedMS;
        public uint PrerequisiteId;
        public uint QuestShareEnum;
        public uint SubMissionPathType;
        public uint LocalizedTextIdCompletedObjectiveShort;
        public uint Quest2SubTypeId;
        public uint LocalizedTextIdMoreInfoSay00;
        public uint LocalizedTextIdMoreInfoSay01;
        public uint LocalizedTextIdMoreInfoSay02;
        public uint LocalizedTextIdMoreInfoSay03;
        public uint LocalizedTextIdMoreInfoSay04;
        public uint LocalizedTextIdMoreInfoText00;
        public uint LocalizedTextIdMoreInfoText01;
        public uint LocalizedTextIdMoreInfoText02;
        public uint LocalizedTextIdMoreInfoText03;
        public uint LocalizedTextIdMoreInfoText04;
        public uint VirtualItemIdPushed00;
        public uint VirtualItemIdPushed01;
        public uint VirtualItemIdPushed02;
        public uint VirtualItemIdPushed03;
        public uint VirtualItemPushedCount00;
        public uint VirtualItemPushedCount01;
        public uint VirtualItemPushedCount02;
        public uint VirtualItemPushedCount03;
        public uint VirtualItemPushedObjectiveFlagsEnum00;
        public uint VirtualItemPushedObjectiveFlagsEnum01;
        public uint VirtualItemPushedObjectiveFlagsEnum02;
        public uint VirtualItemPushedObjectiveFlagsEnum03;
        public uint QuestDirectionIdCompletion;
        [GameTableFieldArray(2u)]
        public uint[] Faction2IdRewardReputations;
        [GameTableFieldArray(2u)]
        public float[] RewardReputationOverrides;
        public uint QuestCategoryId;
        public uint LocalizedTextIdGiverSayDecline;
        public uint PeriodicQuestGroupId;
        public uint PeriodicQuestWeight;
        public uint QuestRepeatPeriodEnum;
        public uint QuestContentFinderTypeEnum;
        public uint GroupSize;
    }
}
