using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetModeDefault)]
    public class SetModeDefaultCommand : IEntityCommandModel
    {
        public bool Unused { get; set; }

        public void Read(GamePacketReader reader)
        {
            Unused = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unused);
        }
    }
}
