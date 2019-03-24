using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerPlayerInfoFullResponse, MessageDirection.Server)]
    public class ServerPlayerInfoFullResponse : IWritable
    {
        public byte Unk0 { get; set; } = 0;
        public ushort Realm { get; set; }
        public ulong CharacterId { get; set; }
        public string Name { get; set; }
        public Faction Faction { get; set; }
        public bool Unk1 { get; set; } = true;
        public Path Path { get; set; }
        public Class Class { get; set; }
        public uint Level { get; set; }
        public bool Unk2 { get; set; } = true;
        public float LastOnlineInDays { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unk0, 3);
            writer.Write(Realm, 14u);
            writer.Write(CharacterId);
            writer.WriteStringFixed(Name);
            writer.Write((ushort)Faction, 14);
            writer.Write(Unk1);
            writer.Write((byte)Path, 3);
            writer.Write((ushort)Class, 14);
            writer.Write(Level);
            writer.Write(Unk2);
            writer.Write(LastOnlineInDays);
        }
    }
}
