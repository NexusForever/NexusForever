namespace NexusForever.GameTable.Model
{
    public class PrimalMatrixNodeEntry
    {
        public uint Id { get; set; }
        public uint PositionX { get; set; }
        public uint PositionY { get; set; }
        public uint PrimalMatrixNodeTypeEnum { get; set; }
        public uint Flags { get; set; }
        public uint MaxAllocations { get; set; }
        public uint CostRedEssence { get; set; }
        public uint CostBlueEssence { get; set; }
        public uint CostGreenEssence { get; set; }
        public uint CostPurpleEssence { get; set; }
        public uint PrimalMatrixRewardIdWarrior { get; set; }
        public uint PrimalMatrixRewardIdEngineer { get; set; }
        public uint PrimalMatrixRewardIdEsper { get; set; }
        public uint PrimalMatrixRewardIdMedic { get; set; }
        public uint PrimalMatrixRewardIdStalker { get; set; }
        public uint PrimalMatrixRewardIdSpellslinger { get; set; }
    }
}
