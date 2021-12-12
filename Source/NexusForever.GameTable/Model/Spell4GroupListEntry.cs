namespace NexusForever.GameTable.Model
{
    public class Spell4GroupListEntry
    {
        public uint Id;
        [GameTableFieldArray(32u)]
        public uint[] SpellGroupIds;
    }
}
