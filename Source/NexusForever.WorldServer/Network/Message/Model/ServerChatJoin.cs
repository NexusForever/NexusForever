using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerChatJoin)]
    public class ServerChatJoin : IWritable
    {
        public Channel Channel { get; set; }
        public string Name { get; set; }
        public uint MemberCount { get; set; }
        public uint Flags { get; set; }
        public uint Order { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Channel.Write(writer);
            writer.WriteStringWide(Name);
            writer.Write(MemberCount);
            writer.Write(Flags);
            writer.Write(Order);
        }
    }
}
