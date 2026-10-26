using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.Bi0635)]
    public class Bi0635 : IWritable
    {
        // TODO: Figure out what to read now that its bidirectional.
        public uint Unknown0 { get; set; } = 0;

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
        }
    }
}
