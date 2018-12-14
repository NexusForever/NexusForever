using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerPathLog, MessageDirection.Server)]
    public class ServerPathLog : IWritable
    {
        public class Progress
        {
            public Progress() { }

            public Progress(uint soldier, uint settler, uint scientist, uint explorer)
            {
                Soldier     = soldier;
                Settler     = settler;
                Scientist   = scientist;
                Explorer    = explorer;
            }

            public uint Soldier { get; set; }
            public uint Settler { get; set; }
            public uint Scientist { get; set; }
            public uint Explorer { get; set; }

            public void Write(GamePacketWriter writer)
            {
                uint[] unlockedArray = new uint[4]{
                    Soldier,
                    Settler,
                    Scientist,
                    Explorer
                };

                for (uint i = 0u; i < unlockedArray.Length; i++)
                    writer.Write(unlockedArray[i]);
            }
        }

        public Path ActivePath { get; set; }
        public Progress PathProgress { get; set; }
        public PathUnlocked UnlockedPathMask { get; set; }
        public uint Unknown3 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ActivePath, 3);

            PathProgress.Write(writer);

            writer.Write(UnlockedPathMask, 4);
            writer.Write(Unknown3);
        }
    }
}
