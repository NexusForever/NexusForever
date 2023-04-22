﻿using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;
using NetworkMessage = NexusForever.Network.Message.Model.Shared.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerRealmList)]
    public class ServerRealmList : IWritable
    {
        public class RealmInfo : IWritable
        {
            public class AccountRealmData : IWritable
            {
                public ushort RealmId { get; set; }
                public uint CharacterCount { get; set; }
                public string LastPlayedCharacter { get; set; }
                public ulong LastPlayedTime { get; set; }

                public void Write(GamePacketWriter writer)
                {
                    writer.Write(RealmId, 14u);
                    writer.Write(CharacterCount);
                    writer.WriteStringWide(LastPlayedCharacter);
                    writer.Write(LastPlayedTime);
                }
            }

            public uint RealmId { get; set; }
            public string RealmName { get; set; }
            public uint Unknown2 { get; set; }
            public uint Unknown3 { get; set; }
            public RealmType Type { get; set; }
            public RealmStatus Status { get; set; }
            public RealmPopulation Population { get; set; }
            public uint Unknown7 { get; set; }
            public byte[] Unknown8 { get; set; }
            public AccountRealmData AccountRealmInfo { get; set; }
            public ushort UnknownC { get; set; }
            public ushort UnknownD { get; set; }
            public ushort UnknownE { get; set; }
            public ushort UnknownF { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(RealmId);
                writer.WriteStringWide(RealmName);
                writer.Write(Unknown2);
                writer.Write(Unknown3);
                writer.Write(Type, 2u);
                writer.Write(Status, 3u);
                writer.Write(Population, 3u);
                writer.Write(Unknown7);
                writer.WriteBytes(Unknown8, 16u);
                AccountRealmInfo.Write(writer);
                writer.Write(UnknownC);
                writer.Write(UnknownD);
                writer.Write(UnknownE);
                writer.Write(UnknownF);
            }
        }

        public ulong Unknown0 { get; set; }
        public List<RealmInfo> Realms { get; set; } = new();
        public List<NetworkMessage> Messages { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
            writer.Write(Realms.Count);
            Realms.ForEach(s => s.Write(writer));
            writer.Write(Messages.Count);
            Messages.ForEach(s => s.Write(writer));
        }
    }
}
