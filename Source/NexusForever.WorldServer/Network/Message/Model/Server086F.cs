using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server086F, MessageDirection.Server)]
    public class Server086F : IWritable
    {
        public uint MountGuid { get; set; }
        public byte Unknown0 { get; set; }
        public byte Unknown1 { get; set; }
        public uint OwnerGuid { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MountGuid);
            writer.Write(Unknown0, 2);
            writer.Write(Unknown1, 3);
            writer.Write(OwnerGuid);
        }
    }
}
