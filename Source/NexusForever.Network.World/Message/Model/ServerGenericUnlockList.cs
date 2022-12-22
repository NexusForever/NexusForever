using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGenericUnlockList)]
    public class ServerGenericUnlockList : IWritable
    {
        public List<uint> Unlocks { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unlocks.Count);
            foreach (uint unlock in Unlocks)
                writer.Write(unlock);
        }
    }
}
