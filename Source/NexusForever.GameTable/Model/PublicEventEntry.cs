using NexusForever.Game.Static.Event;

namespace NexusForever.GameTable.Model
{
    public class PublicEventEntry
    {
        public uint Id;
        public uint WorldId;
        public uint WorldZoneId;
        public uint LocalizedTextIdName;
        public uint FailureTimeMs;
        public uint WorldLocation2Id;
        public PublicEventType PublicEventTypeEnum;
        public uint PublicEventIdParent;
        public uint MinPlayerLevel;
        public uint LiveEventIdLifetime;
        public uint PublicEventFlags;
        public uint LocalizedTextIdEnd;
        public uint RewardRotationContentId;
    }
}
