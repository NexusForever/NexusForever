using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    // Used to adjust the scheduled time cinematic actions will execute based on their Delay + this SequenceOffset.
    // SequenceOffset in this message is always relative to the instanteous current time
    [Message(GameMessageOpcode.ServerCinematicStartTime)]
    public class ServerCinematicSequenceOffset : IWritable
    {
        public uint SequenceOffset { get; set; } 

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SequenceOffset);
        }
    }
}
