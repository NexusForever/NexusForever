using System;
using NexusForever.Shared.Network.Message;

namespace NexusForever.Shared.Network
{
    // UUID structure in Windows
    public class NetworkGuid : IReadable
    {
        public Guid Guid { get; private set; }

        public int Data1 { get; private set; }
        public short Data2 { get; private set; }
        public short Data3 { get; private set; }
        public byte[] Data4 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Data1 = reader.ReadInt();
            Data2 = reader.ReadShort();
            Data3 = reader.ReadShort();
            Data4 = reader.ReadBytes(8u);
            Guid  = new Guid(Data1, Data2, Data3, Data4);
        }
    }
}
