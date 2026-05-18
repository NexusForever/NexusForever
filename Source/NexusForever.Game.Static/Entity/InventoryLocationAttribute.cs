namespace NexusForever.Game.Static.Entity
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InventoryLocationAttribute : Attribute
    {
        public uint DefaultCapacity { get; }

        public InventoryLocationAttribute(uint defaultCapacity)
        {
            DefaultCapacity = defaultCapacity;
        }
    }
}
