namespace NexusForever.Network.World.Entity.Model
{
    public class HousingMannequinEntityModel : IEntityModel
    {
        public uint CreatureId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18u);
        }
    }
}
