using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCharacterSelectFail)]
    public class ServerCharacterSelectFail : IWritable
    {
        public CharacterSelectResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 3u);
        }
    }
}
