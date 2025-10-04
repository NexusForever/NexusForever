using System.Runtime.InteropServices;
using NexusForever.Game.Static.Guild;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
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

        public Identity GuildIdentity { get; private set; } = new();
        public uint Rank { get; private set; }
        public OperationData Data { get; private set; }
        public string TextValue { get; private set; } // Client has this named as "Who", but it is used for Player Names, Rank Names, Notes, MOTD, and other stuff
        public GuildOperation Operation { get; private set; }

        public void Read(GamePacketReader reader)
        {
            GuildIdentity.Read(reader);
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
