using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerChangeActiveActionSet, MessageDirection.Server)]
    public class ServerChangeActiveActionSet : IWritable
    {
        public byte ActionSetIndex { get; set; }
        public uint ActionSetError { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ActionSetIndex, 3u);
            writer.Write(ActionSetError);
        }
    }
}
