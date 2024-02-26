namespace NexusForever.GameTable.Model
{
    public class PublicEventObjectiveGatherResourceEntry
    {
        public uint Id { get; set; }
        public uint PublicEventObjectiveGatherResourceEnumFlag { get; set; }
        public uint Creature2IdContainer { get; set; }
        public uint Creature2IdResource { get; set; }
        public uint Spell4IdResource { get; set; }
        public uint Creature2IdStolenResource { get; set; }
        public uint Spell4IdStolenResource { get; set; }
        public uint PublicEventObjectiveGatherResourceIdOpposing { get; set; }
    }
}
