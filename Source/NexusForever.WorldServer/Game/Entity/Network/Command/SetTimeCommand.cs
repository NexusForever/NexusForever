using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetTime)]
    public class SetTimeCommand : IEntityCommandModel
    {
        public uint Time { get; set; }

        public void Read(GamePacketReader reader)
        {
            Time = reader.ReadUInt();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Time);
        }
    }
}
