using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetMoveDefaults)]
    public class SetMoveDefaultsCommand : IEntityCommand
    {
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            Blend = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Blend);
        }
    }
}
