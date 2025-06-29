using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    // Sent when client makes LAS change request (0xB1) and before the new LAS is sent (0x19D).
    [Message(GameMessageOpcode.ServerActionSetClearCache)]
    public class ServerActionSetClearCache : IWritable
    {
        public bool GenerateChatLogMessage { get; set; } = true; // Sniffs seem all set to true

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GenerateChatLogMessage);
        }
    }
}
