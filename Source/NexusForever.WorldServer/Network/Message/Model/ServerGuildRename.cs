using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
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
