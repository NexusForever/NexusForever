using NexusForever.Game.Static.Setting;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
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
