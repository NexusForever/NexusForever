using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity.Network.Model
{
    public class PlayerEntityModel : IEntityModel
    {
        public ulong Id { get; set; }
        public ushort Unknown8 { get; set; }
        public string Name { get; set; }
        public Race Race { get; set; }
        public Class Class { get; set; }
        public Sex Sex { get; set; }
        public Path Path { get; set; }
        public ulong Unknown20 { get; set; }
        public List<uint> Unknown2C { get; } = new List<uint>();
        public string Unknown30 { get; set; }
        public byte Unknown34 { get; set; }
        public List<ulong> Unknown3C { get; } = new List<ulong>();
        public List<float> Bones { get; set; } = new List<float>();
        public byte Unknown48 { get; set; }
        public byte Unknown4C { get; set; }
        public ushort Unknown50 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Id);
            writer.Write(Unknown8, 14);
            writer.WriteStringWide(Name);
            writer.Write(Race, 5);
            writer.Write(Class, 5);
            writer.Write(Sex, 2);
            writer.Write(Unknown20);

            writer.Write((byte)Unknown2C.Count);
            Unknown2C.ForEach(e => writer.Write(e));

            writer.WriteStringWide(Unknown30);
            writer.Write(Unknown34, 4);

            writer.Write((byte)Unknown3C.Count, 5);
            Unknown3C.ForEach(e => writer.Write(e));
            writer.Write(Bones.Count, 6);
            Bones.ForEach(e => writer.Write(e));

            writer.Write(Unknown48, 3);
            writer.Write(Unknown4C);
            writer.Write(Unknown50, 14);
        }
    }
}
