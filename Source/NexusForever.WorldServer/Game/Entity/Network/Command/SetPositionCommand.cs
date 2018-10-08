using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetPosition)]
    public class SetPositionCommand : IEntityCommand
    {
        public Position Position { get; set; }
        public bool Unknown3 { get; set; }

        public void Read(GamePacketReader reader)
        {
            Position = new Position();
            Position.Read(reader);
            Unknown3 = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            Position.Write(writer);
            writer.Write(Unknown3);
        }
    }
}
