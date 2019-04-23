using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerChangeActiveActionSet)]
    public class ServerChangeActiveActionSet : IWritable
    {
        public byte ActionSetIndex { get; set; }
        public SpecError SpecError { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ActionSetIndex, 3u);
            writer.Write(SpecError, 32u);
        }
    }
}
