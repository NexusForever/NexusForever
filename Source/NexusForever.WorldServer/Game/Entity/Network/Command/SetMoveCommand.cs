using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetMove)]
    public class SetMoveCommand : IEntityCommand
    {
        public Move MoveData { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            MoveData = new Move();
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
