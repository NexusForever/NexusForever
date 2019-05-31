using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerTutorial)]
    public class ServerTutorial : IWritable
    {
        public uint TutorialId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TutorialId);
        }
    }
}
