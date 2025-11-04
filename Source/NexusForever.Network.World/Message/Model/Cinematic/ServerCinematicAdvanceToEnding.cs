using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // TODO:: confirm
    [Message(GameMessageOpcode.ServerCinematicAdvanceToEnding)] 
    public class ServerCinematicAdvanceToEnding : IWritable
    {
        public byte Unused { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unused);
        }
    }
}
