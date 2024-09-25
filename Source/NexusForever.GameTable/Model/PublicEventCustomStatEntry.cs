using NexusForever.Game.Static.Event;

namespace NexusForever.GameTable.Model
{
    public class PublicEventCustomStatEntry
    {
        public uint Id;
        public PublicEventType PublicEventTypeEnum;
        public uint PublicEventId;
        public uint StatIndex;
        public uint LocalizedTextIdStatName;
        public string IconPath;
    }
}
