using System;
using System.Linq;

namespace NexusForever.Shared.Cryptography
{
    public class Arc4Provider
    {
        private uint i;
        private uint j;
        private readonly byte[] s;

        public Arc4Provider(byte[] key)
        {
            i = 0;
            j = 0;
            s = Enumerable.Range(0, 256)
                .Select(i => (byte)i)
                .ToArray();

            for (uint index = 0u, index2 = 0u; index < 256u; index++)
            {
                index2 = (index2 + key[index % key.Length] + s[index]) & 0xFF;
                Swap(s, index, index2);
            }
        }

        public void Encrypt(byte[] buffer)
        {
            CycleBuffer(buffer);
        }

        public void Decrypt(byte[] buffer)
        {
            CycleBuffer(buffer);
        }

        private void CycleBuffer(byte[] buffer)
        {
            for (uint index = 0u; index < buffer.Length; index++)
            {
                i = (i + 1) & 0xFF;
                j = (j + s[i]) & 0xFF;
                Swap(s, i, j);

                buffer[index] = (byte)((buffer[index]) ^ s[(s[j] + s[i]) & 0xFF]);
            }
        }

        private static void Swap(byte[] s, uint i, uint j)
        {
            byte swap = s[i];
            s[i] = s[j];
            s[j] = swap;
        }
    }
}
