using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Increments the AcceptedAlly count and decrements the UnacceptedAlly count if true
    // Increments the AcceptedEnemy count and decrements the UnacceptedEnemy count if false
    [Message(GameMessageOpcode.ServerMatchingMatchParticipantCountUpdate)]
    public class ServerMatchingMatchParticipantCountUpdate : IWritable
    {
        public bool Ally { get; set; } // true = ally, false = enemy

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Ally);
        }
    }
}
