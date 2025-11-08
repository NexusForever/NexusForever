namespace NexusForever.Network.World.Entity.Model
{
    public class HousingPlantEntityModel : IEntityModel
    {
        public ushort Creature2Id { get; set; }
        public ushort WorldSocketId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Creature2Id, 18u);
            writer.Write(WorldSocketId, 14u);
        }
    }
}
