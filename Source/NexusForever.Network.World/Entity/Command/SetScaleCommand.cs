namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetScale)]
    public class SetScaleCommand : IEntityCommandModel
    {
        public ushort Scale { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            Scale = reader.ReadUShort();
            Blend = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Scale);
            writer.Write(Blend);
        }
    }
}
