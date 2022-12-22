using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPlayerInfoFullResponse)]
    public class ServerPlayerInfoFullResponse : IWritable
    {
        public Base BaseData { get; set; }

        public class Base : IWritable
        {
            public byte ResultCode { get; set; }
            public TargetPlayerIdentity Identity { get; set; }
            public string Name { get; set; }
            public Faction Faction { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(ResultCode, 3u);
                Identity.Write(writer);
                writer.WriteStringFixed(Name);
                writer.Write(Faction, 14u);
            }
        }

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
