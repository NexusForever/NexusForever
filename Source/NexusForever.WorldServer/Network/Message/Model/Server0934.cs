using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server0934, MessageDirection.Server)]
    public class Server0934 : IWritable
    {
        public uint MountGuid { get; set; }
        public ushort Faction { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MountGuid);
            writer.Write(Faction, 14);
        }
    }
}
