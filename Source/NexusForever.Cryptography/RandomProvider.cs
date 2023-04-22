using System.Numerics;
using System.Security.Cryptography;

namespace NexusForever.Cryptography
{
    public static class RandomProvider
    {
        public static byte[] GetBytes(uint count)
        {
            return RandomNumberGenerator.GetBytes((int)count);
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
