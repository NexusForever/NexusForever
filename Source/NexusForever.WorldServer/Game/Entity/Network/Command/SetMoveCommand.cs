using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetMove)]
    public class SetMoveCommand : IEntityCommandModel
    {
        public Move MoveData { get; set; } = new Move();
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            MoveData.Read(reader);
            Blend = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            MoveData.Write(writer);
            writer.Write(Blend);
        }
    }
}
