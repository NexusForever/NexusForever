namespace NexusForever.Network.World.Entity.Model
{
    public class NonPlayerEntityModel : IEntityModel
    {
        public uint CreatureId { get; set; }
        public byte QuestChecklistIdx { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18);
            writer.Write(QuestChecklistIdx);
        }
    }
}
