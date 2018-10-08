using System.Runtime.InteropServices;

namespace NexusForever.Shared.GameTable
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GameTableHeader
    {
        public uint Signature;
        public uint Version;
        public ulong NameLength;
        public ulong Unknown1;
        public ulong RecordSize;
        public ulong FieldCount;
        public ulong FieldOffset;
        public ulong RecordCount;
        public ulong TotalRecordSize;
        public ulong RecordOffset;
        public ulong MaxId;
        public ulong LookupOffset;
        public ulong Unknown2;
    }
}
