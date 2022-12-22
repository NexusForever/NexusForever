using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGuildRename)]
    public class ServerGuildRename : IWritable
    {
        public TargetGuild TargetGuild { get; set; }
        public string Name { get; set; }

        public void Write(GamePacketWriter writer)
        {
            TargetGuild.Write(writer);
            writer.WriteStringWide(Name);
        }
    }
}
