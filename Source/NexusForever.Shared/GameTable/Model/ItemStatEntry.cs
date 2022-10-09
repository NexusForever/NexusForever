using NexusForever.Shared.GameTable.Static;

namespace NexusForever.Shared.GameTable.Model
{
    public class ItemStatEntry
    {
        public uint Id;
        [GameTableFieldArray(5)]
        public ItemStatType[] ItemStatTypeEnum;
        [GameTableFieldArray(5)]
        public uint[] ItemStatData;
    }
}
