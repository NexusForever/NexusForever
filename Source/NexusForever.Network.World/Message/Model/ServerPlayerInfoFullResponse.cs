using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPlayerInfoFullResponse)]
    public class ServerPlayerInfoFullResponse : IWritable
    {
        public ServerPlayerInfoBasicResponse BaseData { get; set; }

        public bool IsClassPathSet { get; set; } = true;
        public Path Path { get; set; }
        public Class Class { get; set; }
        public uint Level { get; set; }
        public bool IsLastLoggedOnInDaysSet { get; set; }
        public float LastLoggedInDays { get; set; }

        public void Write(GamePacketWriter writer)
        {
            BaseData.Write(writer);
            writer.Write(IsClassPathSet);
            writer.Write(Path, 3u);
            writer.Write(Class, 14u);
            writer.Write(Level);
            writer.Write(IsLastLoggedOnInDaysSet);
            writer.Write(LastLoggedInDays);
        }
    }
}
