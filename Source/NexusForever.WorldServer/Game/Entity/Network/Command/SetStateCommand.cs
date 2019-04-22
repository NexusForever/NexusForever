using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetState)]
    public class SetStateCommand : IEntityCommandModel
    {
        public uint State { get; set; }

        public void Read(GamePacketReader reader)
        {
            State = reader.ReadUInt();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(State);
        }
    }
}
