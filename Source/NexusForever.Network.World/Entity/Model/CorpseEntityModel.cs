namespace NexusForever.Network.World.Entity.Model
{
    public class CorpseEntityModel : IEntityModel
    {
        public uint CreatureId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18u);
        }
    }
}
