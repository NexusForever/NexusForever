using System.Runtime.InteropServices;

namespace NexusForever.Shared.GameTable
{
    public enum Language
    {
        English = 1,
        German,
        French,
        Korean
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TextTableHeader
    {
        public uint Signature;
        public uint Version;
        public Language Language;
        public uint Unknown1;
        public ulong TagNameLength;
        public ulong TagNameOffset;
        public ulong ShortNameLength;
        public ulong ShortNameOffset;
        public ulong LongNameLength;
        public ulong LongNameOffset;
        public ulong RecordCount;
        public ulong RecordOffset;
        public ulong NameCount;
        public ulong NameOffset;
    }
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
