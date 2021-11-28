using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
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
