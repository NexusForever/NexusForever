using System;

namespace NexusForever.Shared.Network
{
    public class NetworkBitArray
    {
        private readonly byte[] buffer;

        /// <summary>
        /// Initialise a new <see cref="NetworkBitArray"/> with supplied bit size.
        /// </summary>
        public NetworkBitArray(uint size)
        {
            buffer = new byte[(size - 1) / 8 + 1];
        }

        /// <summary>
        /// Set bit at supplied position to value.
        /// </summary>
        public void SetBit(uint position, bool value)
        {
            uint index = position / 8;
            if (index > buffer.Length)
                throw new ArgumentOutOfRangeException();

            uint offset = 7 - (position % 8);
            buffer[index] |= (byte)(1 << (int)offset);
        }

        /// <summary>
        /// Return underlying byte array.
        /// </summary>
        public byte[] GetBuffer()
        {
            return buffer;
        }
    }
}
