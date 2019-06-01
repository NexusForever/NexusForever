using System;

namespace NexusForever.Shared.Network
{
    public class NetworkBitArray
    {
        public enum BitOrder
        {
            LeastSignificantBit,
            MostSignificantBit
        }

        private readonly byte[] buffer;
        private readonly BitOrder order;

        /// <summary>
        /// Initialise a new <see cref="NetworkBitArray"/> with supplied bit size.
        /// </summary>
        public NetworkBitArray(uint size, BitOrder order)
        {
            buffer = new byte[(size - 1) / 8 + 1];
            this.order = order;
        }

        /// <summary>
        /// Set bit at supplied position to value.
        /// </summary>
        public void SetBit(uint position, bool value)
        {
            uint index = position / 8;
            if (index > buffer.Length)
                throw new ArgumentOutOfRangeException();

            uint offset = order == BitOrder.LeastSignificantBit ? position % 8 : 7 - (position % 8);
            buffer[index] |= (byte)((value ? 1u : 0u) << (int)offset);
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
