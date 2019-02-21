using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerUpdateHealth)]
    public class ServerUpdateHealth : IWritable
    {
        public uint UnitId { get; set; }
        public uint Health { get; set; }
        // update reasons or something
        public uint UnknownMask { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Health);
            writer.Write(UnknownMask, 18);
        }
    }
}
