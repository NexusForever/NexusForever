namespace NexusForever.GameTable.Model
{
    public class EntitlementEntry
    {
        public uint Id { get; set; }
        public uint MaxCount { get; set; }
        public uint Flags { get; set; }
        public uint Spell4IdPersistentBuff { get; set; }
        public uint CharacterTitleId { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint LocalizedTextIdDescription { get; set; }
        public string ButtonIcon { get; set; }
    }
}
