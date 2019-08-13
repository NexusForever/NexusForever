using System.Runtime.InteropServices;
using NexusForever.Shared.GameTable.Static;

namespace NexusForever.Shared.GameTable
{
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
        public ulong StringTableLength;
        public ulong StringTableOffset;
    }
}
