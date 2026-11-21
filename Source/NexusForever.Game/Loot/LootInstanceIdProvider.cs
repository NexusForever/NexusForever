using NexusForever.Game.Abstract.Loot;

namespace NexusForever.Game.Loot
{
    public class LootInstanceIdProvider : ILootInstanceIdProvider
    {
        private uint id = 1;

        public uint GetId()
        {
            return id++;
        }
    }
}
