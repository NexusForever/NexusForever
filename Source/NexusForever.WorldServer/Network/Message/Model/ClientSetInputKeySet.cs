using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Setting.Static;

namespace NexusForever.WorldServer.Network.Message.Model
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
