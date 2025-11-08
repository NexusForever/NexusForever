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
        public List<INetworkEntityCommand> Commands { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
            writer.Write(Time);
            writer.Write(TimeReset);
            writer.Write(ServerControlled);

            writer.Write((byte)Commands.Count, 5u);
            foreach (INetworkEntityCommand command in Commands)
            {
                writer.Write(command.Command, 5);
                command.Model.Write(writer);
            }
        }
    }
}
