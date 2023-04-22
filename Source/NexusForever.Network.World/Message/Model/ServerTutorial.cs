using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
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
