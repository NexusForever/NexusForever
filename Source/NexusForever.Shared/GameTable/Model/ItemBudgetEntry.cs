namespace NexusForever.Shared.GameTable.Model
{
    public class ItemBudgetEntry
    {
        public uint Id;
        [GameTableFieldArray(5)]
        public float[] Budgets;
    }
}
