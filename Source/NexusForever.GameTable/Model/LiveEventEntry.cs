namespace NexusForever.GameTable.Model
{
    public class LiveEventEntry
    {
        public uint Id { get; set; }
        public uint LiveEventTypeEnum { get; set; }
        public uint MaxValue { get; set; }
        public uint Flags { get; set; }
        public uint LiveEventCategoryEnum { get; set; }
        public uint LiveEventIdParent { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint LocalizedTextIdSummary { get; set; }
        public string IconPath { get; set; }
        public string IconPathButton { get; set; }
        public string SpritePathTitle { get; set; }
        public string SpritePathBackground { get; set; }
        public uint CurrencyTypeIdEarned { get; set; }
        public uint LocalizedTextIdCurrencyEarnedTooltip { get; set; }
        public uint WorldLocation2IdExile { get; set; }
        public uint WorldLocation2IdDominion { get; set; }
    }
}
