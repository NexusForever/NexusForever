namespace NexusForever.GameTable.Model
{
    public class PublicEventVoteEntry
    {
        public uint Id;
        public uint LocalizedTextIdTitle;
        public uint LocalizedTextIdDescription;
        [GameTableFieldArray(5u)]
        public uint[] LocalizedTextIdOption;
        [GameTableFieldArray(5u)]
        public uint[] LocalizedTextIdLabel;
        public uint DurationMS;
        public string AssetPathSprite;
    }
}
