using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetStateDefault)]
    public class SetStateDefaultCommand : IEntityCommandModel
    {
        public bool Strafe { get; set; }

        public void Read(GamePacketReader reader)
        {
            Strafe = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Strafe);
        }
    }
}
