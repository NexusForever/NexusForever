namespace NexusForever.Network.World.Entity.Model
{
    public class ChestEntityModel : IEntityModel
    {
        public uint CreatureId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18u);
        }
    }
}
