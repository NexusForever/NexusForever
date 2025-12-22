using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    // Not sure what this is for. Player must not be a ghost and this quantity must be greater than 0 for the message to be sent
    // Somehow related to world chunks
    [Message(GameMessageOpcode.Client0xFB)]
    public class Client0xFB : IReadable
    {
        public float Unknown { get; private set; } 

        public void Read(GamePacketReader reader)
        {
            Unknown = reader.ReadSingle();
        }
    }
}
