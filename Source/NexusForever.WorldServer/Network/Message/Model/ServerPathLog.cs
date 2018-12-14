using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerPathLog, MessageDirection.Server)]
    public class ServerPathLog : IWritable
    {
        public Path ActivePath { get; set; }
        public uint[] PathProgress { get; set;  } = new uint[4]; // Total Path XP for each path, in order: Soldier, Settler, Scientist, Explorer
        public PathUnlocked UnlockedPathMask { get; set; }
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
