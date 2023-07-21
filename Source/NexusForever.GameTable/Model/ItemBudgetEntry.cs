namespace NexusForever.GameTable.Model
{
    public class ItemBudgetEntry
    {
        public uint Id;
        [GameTableFieldArray(5u)]
        public float[] Budgets;
    }
}
