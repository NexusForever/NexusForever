namespace NexusForever.Network.World.Entity.Model
{
    public class WorldUnitEntityModel : IEntityModel
    {
        public bool Unknown0 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
        }
    }
}
