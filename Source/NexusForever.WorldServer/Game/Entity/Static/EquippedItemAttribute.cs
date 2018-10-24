using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EquippedItemAttribute : Attribute
    {
        public EquippedItem Slot { get; }

        public EquippedItemAttribute(EquippedItem slot)
        {
            Slot = slot;
        }
    }
}
