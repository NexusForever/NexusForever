using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
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
