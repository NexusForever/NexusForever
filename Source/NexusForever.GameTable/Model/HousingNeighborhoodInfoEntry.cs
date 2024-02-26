namespace NexusForever.GameTable.Model
{
    public class HousingNeighborhoodInfoEntry
    {
        public uint Id { get; set; }
        public uint BaseCost { get; set; }
        public uint MaxPopulation { get; set; }
        public uint PopulationThreshold { get; set; }
        public uint HousingFactionEnum { get; set; }
        public uint HousingFeatureTypeEnum { get; set; }
        public uint HousingPlaystyleTypeEnum { get; set; }
        public uint HousingMapInfoIdPrimary { get; set; }
        public uint HousingMapInfoIdSecondary { get; set; }
    }
}
