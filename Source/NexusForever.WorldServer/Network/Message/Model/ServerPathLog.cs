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
        /**
        * Unlocked Path Masks
        * Soldier = 1
        * Settler = 2
        * Scientist = 4
        * Explorer = 8
        * Soldier, Settler = 3
        * Soldier, Scientist = 5
        * Settler, Scientist = 6
        * Soldier, Scientist, Settler = 7
        * Explorer, Soldier = 9
        * Explorer, Settler = 10
        * Explorer, Soldier, Settler = 11
        * Explorer, Scientist = 12
        * Explorer, Soldier, Scientist = 13
        * Explorer, Settler, Scientist = 14
        * All = 15
        **/
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
