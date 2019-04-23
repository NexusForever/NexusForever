using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Network;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityCommand)]
    public class ServerEntityCommand : IWritable
    {
        public uint Guid { get; set; }
        public uint Time { get; set; }
        public bool TimeReset { get; set; }
        public bool ServerControlled { get; set; }
        public List<(EntityCommand, IEntityCommandModel)> Commands { get; set; } = new List<(EntityCommand, IEntityCommandModel)>();

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
