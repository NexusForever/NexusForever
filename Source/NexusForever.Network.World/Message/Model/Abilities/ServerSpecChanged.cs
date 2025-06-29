using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    [Message(GameMessageOpcode.ServerSpecChanged)]
    public class ServerSpecChanged : IWritable
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
