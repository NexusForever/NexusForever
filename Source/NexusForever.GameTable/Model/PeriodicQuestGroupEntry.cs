namespace NexusForever.GameTable.Model
{
    public class PeriodicQuestGroupEntry
    {
        public uint Id { get; set; }
        public uint PeriodicQuestSetId { get; set; }
        public uint PeriodicQuestsOffered { get; set; }
        public uint MaxPeriodicQuestsAllowed { get; set; }
        public uint Weight { get; set; }
        public uint ContractTypeEnum { get; set; }
        public uint ContractQualityEnum { get; set; }
    }
}
