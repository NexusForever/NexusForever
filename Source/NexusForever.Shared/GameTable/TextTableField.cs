using System.Runtime.InteropServices;

namespace NexusForever.Shared.GameTable
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TextTableField
    {
        public uint Id;
        public uint Offset;
    }
}
