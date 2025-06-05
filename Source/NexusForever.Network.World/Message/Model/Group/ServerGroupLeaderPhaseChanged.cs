using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupLeaderPhaseChanged)]
    public class ServerGroupLeaderPhaseChanged : IWritable
    {
        public uint ReferenceType { get; set; } // Not sure what the data means
        public uint ReferenceId { get; set; } // Not sure what the data means
        public bool JoinAllowed { get; set; } // Player can join leader in their phase

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ReferenceType);
            writer.Write(ReferenceId);
            writer.Write(JoinAllowed);
        }
    }
}
