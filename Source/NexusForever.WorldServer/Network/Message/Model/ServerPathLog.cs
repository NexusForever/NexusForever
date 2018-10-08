using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerPathLog, MessageDirection.Server)]
    public class ServerPathLog : IWritable
    {
        public byte ActivePath { get; set; }
        public uint[] PathProgress { get; } = new uint[4];
        public byte UnlockedPathMask { get; set; }
        public uint Unknown3 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ActivePath, 3);

            for (uint i = 0u; i < PathProgress.Length; i++)
                writer.Write(PathProgress[i]);

            writer.Write(UnlockedPathMask, 4);
            writer.Write(Unknown3);
        }
    }
}
