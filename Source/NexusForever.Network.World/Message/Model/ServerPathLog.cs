using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathLog)]
    public class ServerPathLog : IWritable
    {

        public Path ActivePath { get; set; }
        public uint[] PathProgress { get; set; } = new uint[4];
        public PathUnlockedMask PathUnlockedMask { get; set; }
        public float TimeSinceLastActivateInDays { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ActivePath, 3);

            for (uint i = 0u; i < PathProgress.Length; i++)
                writer.Write(PathProgress[i]);

            writer.Write(PathUnlockedMask, 4);
            writer.Write(TimeSinceLastActivateInDays);
        }
    }
}
