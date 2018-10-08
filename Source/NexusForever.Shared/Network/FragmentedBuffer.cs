using System;

namespace NexusForever.Shared.Network
{
    public class FragmentedBuffer
    {
        public bool IsComplete => data.Length == offset;

        public byte[] Data
        {
            get
            {
                if (!IsComplete)
                    throw new InvalidOperationException();
                return data;
            }
        }

        private uint offset;
        private readonly byte[] data;

        public FragmentedBuffer(uint size)
        {
            data = new byte[size];
        }

        public void Populate(GamePacketReader reader)
        {
            if (IsComplete)
                throw new InvalidOperationException();

            uint remaining = reader.BytesRemaining;
            if (remaining < data.Length - offset)
            {
                // not enough data, push entire frame into packet
                byte[] newData = reader.ReadBytes(remaining);
                Buffer.BlockCopy(newData, 0, data, (int)offset, (int)remaining);

                offset += (uint)newData.Length;
            }
            else
            {
                // enough data, push required frame into packet
                byte[] newData = reader.ReadBytes((uint)data.Length - offset);
                Buffer.BlockCopy(newData, 0, data, (int)offset, newData.Length);

                offset += (uint)newData.Length;
            }
        }
    }
}
