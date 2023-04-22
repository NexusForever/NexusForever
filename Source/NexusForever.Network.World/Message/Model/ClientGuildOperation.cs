using NexusForever.Network.Message;
using System.Runtime.InteropServices;
using NexusForever.Game.Static.Guild;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGuildOperation)]
    public class ClientGuildOperation : IReadable
    {
        [StructLayout(LayoutKind.Explicit, Size = sizeof(ulong))]
        public struct OperationData
        {
            [FieldOffset(0)]
            public bool BoolData;
            [FieldOffset(0)]
            public int Int32Data;
            [FieldOffset(0)]
            public uint UInt32Data;
            [FieldOffset(0)]
            public ulong UInt64Data;
        }

        public ushort RealmId { get; private set; }
        public ulong GuildId { get; private set; }
        public uint Rank { get; private set; }
        public OperationData Data { get; private set; }
        public string TextValue { get; private set; } // Client has this named as "Who", but it is used for Player Names, Rank Names, Notes, MOTD, and other stuff
        public GuildOperation Operation { get; private set; }

        public void Read(GamePacketReader reader)
        {
            RealmId   = reader.ReadUShort(14u);
            GuildId   = reader.ReadULong();
            Rank      = reader.ReadUInt();
            Data      = new OperationData
            {
                UInt64Data = reader.ReadULong()
            };
            TextValue = reader.ReadWideString();
            Operation = reader.ReadEnum<GuildOperation>(6u);
        }
    }
}
