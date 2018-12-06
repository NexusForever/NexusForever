using System;
using System.Numerics;
using System.Security.Cryptography;

namespace NexusForever.Shared.Cryptography
{
    public static class RandomProvider
    {
        public static byte[] GetBytes(uint count)
        {
            using (var provider = new RNGCryptoServiceProvider())
            {
                byte[] buffer = new byte[count];
                provider.GetBytes(buffer);
                return buffer;
            }
        }

        public static Guid GetGuid()
        {
            byte[] data = GetBytes(16u);
            return new Guid(data);
        }

        public static BigInteger GetUnsignedBigInteger(uint count)
        {
            byte[] data = GetBytes(count);
            return new BigInteger(data, true);
        }
    }
}
