namespace NexusForever.GameTable.Model
{
    public class UnitRaceEntry
    {
        public uint Id { get; set; }
        public uint SoundImpactDescriptionIdOrigin { get; set; }
        public uint SoundImpactDescriptionIdTarget { get; set; }
        public uint UnitVisualTypeId { get; set; }
        public uint SoundEventIdAggro { get; set; }
        public uint SoundEventIdAware { get; set; }
        public uint SoundSwitchIdModel { get; set; }
        public uint SoundCombatLoopId { get; set; }
    }
}
