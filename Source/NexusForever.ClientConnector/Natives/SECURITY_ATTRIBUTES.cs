using System;

namespace NexusForever.ClientConnector
{
    public struct SECURITY_ATTRIBUTES
    {
        public int length;
        public IntPtr lpSecurityDescriptor;
        public bool bInheritHandle;
    }
}
