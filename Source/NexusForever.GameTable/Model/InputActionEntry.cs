namespace NexusForever.GameTable.Model
{
    public class InputActionEntry
    {
        public uint Id { get; set; }
        public string EnumName { get; set; }
        public uint LocalizedTextId { get; set; }
        public uint InputActionCategoryId { get; set; }
        public bool CanHaveUpDownState { get; set; }
        public uint DisplayIndex { get; set; }
    }
}
