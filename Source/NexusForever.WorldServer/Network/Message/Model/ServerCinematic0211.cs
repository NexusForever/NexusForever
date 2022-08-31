using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematic0211)]
    public class ServerCinematic0211 : IWritable
    {
        public uint Unknown0 { get; set; }
        public uint UnitId { get; set; }
        public uint UnitId1 { get; set; }
        public uint Unknown3 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
            writer.Write(UnitId);
            writer.Write(UnitId1);
            writer.Write(Unknown3);
        }
    }
}
