using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pregame
{
    // Client only processes message when on CharacterSelect screen.
    [Message(GameMessageOpcode.ServerQueueStatus)]
    public class ServerQueueStatus : IWritable
    {
        public uint QueuePosition { get; set; }
        public uint WaitTimeSeconds { get; set; }
        public bool IsGuest { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(QueuePosition);
            writer.Write(WaitTimeSeconds);
            writer.Write(IsGuest);
        }
    }
}
