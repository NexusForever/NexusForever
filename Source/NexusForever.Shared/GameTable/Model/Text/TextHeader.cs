using System.Runtime.InteropServices;

namespace NexusForever.Shared.GameTable.Model.Text
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TextHeader
    {
        public uint Id;
        public uint Offset;
    }
}
