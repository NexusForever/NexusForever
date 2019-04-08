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
        public bool Unknown1 { get; set; }
        public bool Unknown2 { get; set; }
        public List<(EntityCommand, IEntityCommand)> Commands { get; set; } = new List<(EntityCommand, IEntityCommand)>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
            writer.Write(Time);
            writer.Write(Unknown1);
            writer.Write(Unknown2);

            writer.Write((byte)Commands.Count, 5);
            foreach ((EntityCommand id, IEntityCommand command) in Commands)
            {
                writer.Write(id, 5);
                command.Write(writer);
            }
        }
    }
}
