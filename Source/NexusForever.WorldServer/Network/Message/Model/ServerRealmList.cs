using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerRealmList, MessageDirection.Server)]
    public class ServerRealmList : IWritable
    {
        public class RealmInfo : IWritable
        {
            public uint Unknown0 { get; set; }
            public string Realm { get; set; }
            public uint Unknown2 { get; set; }
            public uint Unknown3 { get; set; }
            public RealmType Type { get; set; }
            public RealmStatus Status { get; set; }
            public RealmPopulation Population { get; set; }
            public uint Unknown7 { get; set; }

            // struct
            public ushort Unknown8 { get; set; }
            public uint Unknown9 { get; set; }
            public string UnknownA { get; set; }
            public ulong UnknownB { get; set; }

            public ushort UnknownC { get; set; }
            public ushort UnknownD { get; set; }
            public ushort UnknownE { get; set; }
            public ushort UnknownF { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0);
                writer.WriteStringWide(Realm);
                writer.Write(Unknown2);
                writer.Write(Unknown3);
                writer.Write(Type, 2);
                writer.Write(Status, 3);
                writer.Write(Population, 3);
                writer.Write(Unknown7);
                writer.WriteBytes(new byte[16], 16u);

                writer.Write(Unknown8, 14);
                writer.Write(Unknown9);
                writer.WriteStringWide(UnknownA);
                writer.Write(UnknownB);

                writer.Write(UnknownC);
                writer.Write(UnknownD);
                writer.Write(UnknownE);
                writer.Write(UnknownF);
            }
        }

        public class UnknownStructure1 : IWritable
        {
            public uint Unknown0 { get; set; }
            public List<string> Unknown1 { get; } = new List<string>();

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0);
                writer.Write(Unknown1.Count, 8);
                Unknown1.ForEach(writer.WriteStringWide);
            }
        }

        public ulong Unknown0 { get; set; }
        public List<RealmInfo> Realms { get; } = new List<RealmInfo>();
        public List<UnknownStructure1> Unknown2 { get; } = new List<UnknownStructure1>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
            writer.Write(Realms.Count);
            Realms.ForEach(s => s.Write(writer));
            writer.Write(Unknown2.Count);
            Unknown2.ForEach(s => s.Write(writer));
        }
    }
}
