namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetVelocity)]
    public class SetVelocityCommand : IEntityCommandModel
    {
        public Velocity VelocityData { get; set; } = new();
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            VelocityData.Read(reader);
            Blend = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            VelocityData.Write(writer);
            writer.Write(Blend);
        }
    }
}
