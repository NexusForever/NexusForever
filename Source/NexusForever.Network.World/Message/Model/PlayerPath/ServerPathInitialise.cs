using NexusForever.Game.Static.PlayerPath;
using NexusForever.Network.Message;
using Path = NexusForever.Game.Static.PlayerPath.Path;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    [Message(GameMessageOpcode.ServerPathInitialise)]
    public class ServerPathInitialise : IWritable
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
