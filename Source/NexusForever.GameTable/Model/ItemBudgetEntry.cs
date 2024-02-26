namespace NexusForever.GameTable.Model
{
    public class ItemBudgetEntry
    {
        public uint Id { get; set; }
        [GameTableFieldArray(5u)]
        public float[] Budgets { get; set; }
    }
}
