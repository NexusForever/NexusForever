namespace NexusForever.Shared.GameTable.Model
{
    public class Spell4VisualGroupEntry
    {
        public uint Id;
        [GameTableFieldArray(36u)]
        public uint[] Spell4VisualIdVisuals;
        public uint VisualEffectIdPrimaryCaster;
    }
}
