using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityCommand)]
    public class ServerEntityCommand : IWritable
    {
        public uint Guid { get; set; }
        public uint Time { get; set; }
        public bool TimeReset { get; set; }
        public bool ServerControlled { get; set; }
        public List<(EntityCommand, IEntityCommandModel)> Commands { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
            writer.Write(Time);
            writer.Write(TimeReset);
            writer.Write(ServerControlled);

            writer.Write((byte)Commands.Count, 5u);
            foreach ((EntityCommand id, IEntityCommandModel command) in Commands)
            {
                writer.Write(id, 5);
                command.Write(writer);
            }
        }
    }
}
