namespace NexusForever.GameTable.Model
{
    public class Creature2ActionTextEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdOnEnterCombat00 { get; set; }
        public uint LocalizedTextIdOnEnterCombat01 { get; set; }
        public uint LocalizedTextIdOnEnterCombat02 { get; set; }
        public uint LocalizedTextIdOnEnterCombat03 { get; set; }
        public float ChanceToSayOnEnterCombat { get; set; }
        public uint LocalizedTextIdOnDeath00 { get; set; }
        public uint LocalizedTextIdOnDeath01 { get; set; }
        public uint LocalizedTextIdOnDeath02 { get; set; }
        public uint LocalizedTextIdOnDeath03 { get; set; }
        public float ChanceToSayOnDeath { get; set; }
        public uint LocalizedTextIdOnKill00 { get; set; }
        public uint LocalizedTextIdOnKill01 { get; set; }
        public uint LocalizedTextIdOnKill02 { get; set; }
        public uint LocalizedTextIdOnKill03 { get; set; }
        public float ChanceToSayOnKill { get; set; }
    }
}
