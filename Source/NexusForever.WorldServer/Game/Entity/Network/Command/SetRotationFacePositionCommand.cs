using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetRotationFacePosition)]
    public class SetRotationFacePositionCommand : IEntityCommand
    {
        public Position Position { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            Position = new Position();
            Position.Read(reader);
            Blend = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            Position.Write(writer);
            writer.Write(Blend);
        }
    }
}
