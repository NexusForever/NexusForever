using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathScientistScanbotState)]
    public class ServerPathScientistScanbotState : IWritable
    {
        
        public uint ScanbotUnitId { get; set; } // Sent with 0 to despawn the scanbot.
        public uint ScanbotCooldownMS { get; set; } // Only used when the scanbot is despawned.

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ScanbotUnitId);
            writer.Write(ScanbotCooldownMS);
        }
    }
}
