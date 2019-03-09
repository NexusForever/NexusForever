using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server089B, MessageDirection.Server)]
    public class Server089B : IWritable
    {
        public uint UnitId { get; set; }
        public uint MountGuid { get; set; }
        public byte Unknown0 { get; set; }
        public byte Unknown1 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(MountGuid);
            writer.Write(Unknown0, 2u);
            writer.Write(Unknown1, 3u);
        }
    }
}
