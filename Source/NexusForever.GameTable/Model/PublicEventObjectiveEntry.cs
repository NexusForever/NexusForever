using NexusForever.Game.Static.Event;

namespace NexusForever.GameTable.Model
{
    public class PublicEventObjectiveEntry
    {
        public uint Id;
        public uint PublicEventId;
        public PublicEventObjectiveFlag PublicEventObjectiveFlags;
        public uint PublicEventObjectiveTypeSpecificFlags;
        public uint WorldLocation2Id;
        public PublicEventTeam PublicEventTeamId;
        public uint LocalizedTextId;
        public uint LocalizedTextIdOtherTeam;
        public uint LocalizedTextIdShort;
        public uint LocalizedTextIdOtherTeamShort;
        public PublicEventObjectiveType PublicEventObjectiveTypeEnum;
        public uint Count;
        public uint ObjectId;
        public uint FailureTimeMs;
        public uint TargetGroupIdRewardPane;
        public PublicEventObjectiveCategory PublicEventObjectiveCategoryEnum;
        public uint LiveEventIdCounter;
        public uint PublicEventObjectiveIdParent;
        public uint QuestDirectionId;
        public uint MedalPointValue;
        public uint LocalizedTextIdParticipantAdd;
        public uint LocalizedTextIdStart;
        public uint DisplayOrder;
    }
}
