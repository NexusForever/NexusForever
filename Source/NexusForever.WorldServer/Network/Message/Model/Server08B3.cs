using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server08B3)]
    public class Server08B3 : IWritable
    {
        public uint MountGuid { get; set; }
        public uint Unknown0 { get; set; }
        public bool Unknown1 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MountGuid);
            writer.Write(Unknown0);
            writer.Write(Unknown1);
        }
    }
}
