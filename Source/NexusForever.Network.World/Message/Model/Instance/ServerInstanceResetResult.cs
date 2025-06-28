using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerInstanceResetResult)]
    public class ServerInstanceResetResult : IWritable
    {
        public uint Unused { get; set; } // Probably WorldId but not used by client
        public bool Success { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unused, 15u);
            writer.Write(Success);
        }
    }
}
