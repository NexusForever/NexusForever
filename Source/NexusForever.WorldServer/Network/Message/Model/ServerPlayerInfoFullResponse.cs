using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
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
