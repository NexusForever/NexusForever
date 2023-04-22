namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetMode)]
    public class SetModeCommand : IEntityCommandModel
    {
        public uint Mode { get; set; }

        public void Read(GamePacketReader reader)
        {
            Mode = reader.ReadUInt();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Mode);
        }
    }
}
