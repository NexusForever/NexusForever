using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingMatchParticpantCountUpdate)]

    // Increments the AcceptedAlly count and decrements the UnacceptedAlly count if true
    // Increments the AcceptedEnemy count and decrements the UnacceptedEnemy count if false
    public class ServerMatchingMatchParticpantCountUpdate : IWritable
    {
        public bool Ally { get; set; } // true = ally, false = enemy

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Ally);
        }
    }
}
