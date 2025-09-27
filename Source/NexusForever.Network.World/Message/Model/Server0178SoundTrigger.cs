using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Plays the soundEventId set by dataint0 in GameFormula 1263 which happens to be 0 so no sound is played.
    // Possibly related to ServerSpellRemove (0x7FE).
    // Included for completeness but not useful unless the GameFormula tbl entry is overwritten.
    [Message(GameMessageOpcode.Server_0x178_SoundTrigger)]
    public class Server0178SoundTrigger : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // Zero byte message
        }
    }
}
