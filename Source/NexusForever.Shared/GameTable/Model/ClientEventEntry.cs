namespace NexusForever.Shared.GameTable.Model
{
    public class ClientEventEntry
    {
        public uint Id;
        public string Description;
        public uint WorldId;
        public uint EventTypeEnum;
        public uint EventData;
        public uint PrerequisiteId;
        public uint Priority;
        public uint DelayMS;
        public uint ClientEventActionId;
    }
}
