using NexusForever.Game.Static.Option;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Option
{
    [Message(GameMessageOpcode.ClientSetInputKeySet)]
    public class ClientSetInputKeySet : IReadable
    {
        public InputSets InputKeySetEnum { get; private set; }

        public void Read(GamePacketReader reader)
        {
            InputKeySetEnum = reader.ReadEnum<InputSets>(32u);
        }
    }
}
