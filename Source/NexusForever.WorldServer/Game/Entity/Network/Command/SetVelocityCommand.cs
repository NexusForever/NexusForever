using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetVelocity)]
    public class SetVelocityCommand : IEntityCommand
    {
        public Velocity VelocityData { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            VelocityData = new Velocity();
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
