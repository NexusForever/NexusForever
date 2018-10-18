using System;

namespace NexusForever.WorldServer.Game.Entity.Static
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
