using System;
using System.IO;

namespace NexusForever.Shared.Network
{
    public static class Extensions
    {
        public static uint Remaining(this Stream stream)
        {
            if (stream.Length < stream.Position)
                throw new InvalidOperationException();

            return (uint)(stream.Length - stream.Position);
        }
    }
}
