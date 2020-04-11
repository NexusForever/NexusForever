using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicTransition)]
    public class ServerCinematicTransition : IWritable
    {
        public uint Delay { get; set; }
        public uint Flags { get; set; }
        public uint EndTran { get; set; }
        public ushort TranDurationStart { get; set; }
        public ushort TranDurationMid { get; set; }
        public ushort TranDurationEnd { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(Flags);
            writer.Write(EndTran);
            writer.Write(TranDurationStart);
            writer.Write(TranDurationMid);
            writer.Write(TranDurationEnd);
        }
    }
}
